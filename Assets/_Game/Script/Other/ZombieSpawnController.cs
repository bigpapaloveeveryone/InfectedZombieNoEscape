using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieSpawnController : MonoBehaviour
{
    public int initialZombiesPerWave = 5;
    public int currentZombiesPerWave;

    public float spawnDelay = 0.5f; // Delay giữa mỗi lần spawn zombie trong 1 wave

    public int currentWave = 0;
    public float waveCooldown = 10.0f; // Thời gian cooldown giữa các wave

    public bool inCooldown;
    public float cooldownCounter = 0;

    public List<Zombie> currentZombiesAlive = new List<Zombie>();

    public Text waveOverUI;
    public Text cooldownCounterUI;

    public Zombie z1;
    public Zombie z2;

    private List<Transform> spawnPoints = new List<Transform>(); // Danh sách các điểm spawn

    private void Start()
    {
        // Lấy tất cả con của ZombieSpawnController làm điểm spawn
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child);
        }

        currentZombiesPerWave = initialZombiesPerWave;
        StartNextWave();
    }

    private void Update()
    {
        CheckPlayer();

        // Xóa các zombie đã chết khỏi danh sách
        currentZombiesAlive.RemoveAll(zombie => zombie.isDed);

        // Nếu không còn zombie nào và không đang cooldown → bắt đầu cooldown wave mới
        if (currentZombiesAlive.Count == 0 && !inCooldown)
        {
            StartCoroutine(WaveCooldown());
        }

        // Cập nhật UI cooldown
        cooldownCounter = inCooldown ? cooldownCounter - Time.deltaTime : waveCooldown;
        cooldownCounterUI.text = Mathf.Ceil(cooldownCounter).ToString();
    }

    private void CheckPlayer()
    {
        if (GameManager.Ins.player.hitBox.isDead)
        {
            foreach (Zombie z in currentZombiesAlive)
            {
                z.TurnOffAudioSource();
            }
        }
    }

    private IEnumerator WaveCooldown()
    {
        inCooldown = true;
        waveOverUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(waveCooldown);

        inCooldown = false;
        waveOverUI.gameObject.SetActive(false);
        currentZombiesPerWave *= 2;


        if (!GameManager.Ins.player.hitBox.isDead)
        {
            StartNextWave();
        }
    }

    private void StartNextWave()
    {
        currentZombiesAlive.Clear();
        currentWave++;
        GameManager.Ins.player.wp.SetBullet(10);
        UIManager.Ins.mainCanvas.UpdateCurWave(currentWave);
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < currentZombiesPerWave; i++)
        {
            if (spawnPoints.Count == 0)
            {
                Debug.LogWarning("No spawn points found!");
                yield break;
            }

            // Chọn một vị trí spawn ngẫu nhiên từ các child
            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];

            // Chọn loại zombie ngẫu nhiên
            Zombie zombie = UnityEngine.Random.Range(0, 2) == 0 ? Instantiate(z1, spawnPoint.position, Quaternion.identity) :
                                                                  Instantiate(z2, spawnPoint.position, Quaternion.identity);

            if (zombie != null)
            {
                currentZombiesAlive.Add(zombie);
            }
            else
            {
                Debug.LogWarning("Zombie spawn failed!");
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
