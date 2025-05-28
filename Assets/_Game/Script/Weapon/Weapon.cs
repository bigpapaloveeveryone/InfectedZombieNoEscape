using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;
public class Weapon : MonoBehaviour
{
    [SerializeField] private Camera playerCam;

    [Header("-----Shooting Mode-----")]
    public ShootingMode currentShootingMode;

    // Shooting
    [SerializeField] private float shootingDelay = 0.1f;
    private bool isShooting, readyToShoot = true;
    private bool allowReset = true;

    // Burst Mode
    [SerializeField] private int bulletsPerBurst = 3;
    private int burstBulletsLeft;

    // Spread
    [SerializeField] private float baseSpreadIntensity = 0.01f;
    [SerializeField] private float maxSpreadIntensity = 0.1f;
    [SerializeField] private float spreadIncreaseRate = 0.02f;
    private float currentSpreadIntensity;

    [Header("-----Reload-----")]
    [SerializeField] private float reloadTime;
    [SerializeField] private int magazineSize, bulletLeft;
    
    public bool isReloading;

    [Header("-----Weapon Details-----")]
    [SerializeField] private float bulletVelocity = 30f;
    [SerializeField] private float bulletPrefabLifeTime = 3f;

    [Header("-----Other-----")]
    [SerializeField] private Transform bulletSpawn;
    private Animator anim;
    private ParticleSystem eff;
    private bool isOutOfAmmoSoundPlaying;

    private Button shootBtn;


    private void Awake()
    {
        currentSpreadIntensity = baseSpreadIntensity;
        burstBulletsLeft = bulletsPerBurst;

        bulletLeft = magazineSize;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        eff = GetComponentInChildren<ParticleSystem>();
        Input.multiTouchEnabled = true;

        shootBtn = UIManager.Ins.mainCanvas.GetShootButton();
        if (shootBtn != null)
        {
            // Khi nhấn giữ nút bắn, chỉ kích hoạt bắn nếu không đang nạp đạn
            shootBtn.OnPointerDown(() => {
                if (!isReloading)
                    isShooting = true;
            });
            shootBtn.OnPointerUp(() => isShooting = false);
        }
    }

    void Update()
    {
        if (isReloading)    
            return;

        if (bulletLeft == 0 && isShooting && !isOutOfAmmoSoundPlaying)
        {
            PlayOutOfAmmoSound();
        }

        bool wasShooting = isShooting;
        //isShooting = currentShootingMode == ShootingMode.Auto ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0);

        if (readyToShoot && isShooting && bulletLeft > 0)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
            isOutOfAmmoSoundPlaying = false;
        }

        if (wasShooting && !isShooting)
        {
            ResetSpread();
        }

        UIManager.Ins.mainCanvas.TextAmmo(bulletLeft, bulletsPerBurst, magazineSize);
    }

    public void SetBullet(int amount)
    {
        magazineSize += amount;
    }

    private void PlayOutOfAmmoSound()
    {
        isOutOfAmmoSoundPlaying = true;
        AudioManager.Ins.PlaySFX(AudioManager.Ins.outOfAmmo);

        // Đợi âm thanh chạy hết rồi mới cho phép phát lại
        float clipLength = AudioManager.Ins.outOfAmmo.length;
        Invoke(nameof(ResetOutOfAmmoSound), clipLength);
    }

    private void ResetOutOfAmmoSound()
    {
        isOutOfAmmoSoundPlaying = false;
    }

    public void Reload()
    {
        if (bulletLeft < magazineSize && !isReloading)
        {
            isReloading = true;
            UIManager.Ins.mainCanvas.IsReloading(isReloading);
            Invoke(nameof(ReloadComplete), reloadTime);
        }
    }

    public bool NeedReload()
    {
        return readyToShoot && !isShooting && !isReloading && bulletLeft <= 0;
    }

    public void ReloadComplete()
    {
        Debug.Log("D");
        bulletLeft = magazineSize;
        isReloading = false;
        UIManager.Ins.mainCanvas.IsReloading(isReloading);
    }

    private void FireWeapon()
    {
        
        bulletLeft--;

        AudioManager.Ins.PlaySFX(AudioManager.Ins.shoot);

        eff.Play();
        anim.Play(CacheString.TAG_SHOOT);
        readyToShoot = false;

        CameraShaker.Instance.ShakeOnce(0.6f, 2.5f, 0.07f, 0.15f);

        Vector3 shootingDir = CalculateDirectionAndSpread().normalized;

        Bullet b = SimplePool.Spawn<Bullet>(PoolType.Bullet, bulletSpawn.position, Quaternion.LookRotation(shootingDir));
        b.Shoot(shootingDir, bulletVelocity);

        StartCoroutine(WaitToDestroyCoroutine(bulletPrefabLifeTime, b));

        if (currentShootingMode == ShootingMode.Auto)
        {
            currentSpreadIntensity = Mathf.Min(currentSpreadIntensity + spreadIncreaseRate, maxSpreadIntensity);
        }

        if (allowReset)
        {
            Invoke(nameof(ResetShot), shootingDelay);
            allowReset = false;
        }

        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke(nameof(FireWeapon), shootingDelay);
        }
    }

    private void ResetSpread()
    {
        currentSpreadIntensity = baseSpreadIntensity;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    private IEnumerator WaitToDestroyCoroutine(float time, Bullet b)
    {
        yield return new WaitForSeconds(time);

        if (b == null)
            yield break;    
        SimplePool.Despawn(b);
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        // Lấy hướng từ camera (giữa màn hình) đến điểm mà ray bắn được
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint = Physics.Raycast(ray, out hit) ? hit.point : ray.GetPoint(100f);
        Vector3 baseDirection = (targetPoint - playerCam.transform.position).normalized;

        // Tạo offset ngẫu nhiên theo chiều phải và trên của camera
        float xOffset = Random.Range(-currentSpreadIntensity, currentSpreadIntensity);
        float yOffset = Random.Range(-currentSpreadIntensity, currentSpreadIntensity);

        // Tạo vector offset
        Vector3 offset = playerCam.transform.right * xOffset + playerCam.transform.up * yOffset;

        // Thêm offset vào hướng cơ bản và sau đó chuẩn hóa lại
        Vector3 finalDirection = (baseDirection + offset).normalized;
        return finalDirection;
    }

    public bool CheckIsShooting()
    {
        return isShooting;
    }
}

public enum ShootingMode
{
    Single,
    Burst,
    Auto
}
