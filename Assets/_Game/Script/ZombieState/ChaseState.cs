using System.Collections;
using UnityEngine;

public class ChaseState : IState<Zombie>
{
    private AudioSource audioSource;
    private Transform player;
    private Zombie bot;
    private Coroutine chaseSoundCoroutine;

    public void OnEnter(Zombie bot)
    {
        this.bot = bot;

        bot.navMeshAgent.isStopped = false; // Cho phép di chuyển
        bot.PlayAnim(CacheString.TAG_ZRUN);
        Debug.Log("Chase State");

        audioSource = bot.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("Zombie is missing an AudioSource!");
            return;
        }

        player = GameManager.Ins.player.transform;

        // Nếu có coroutine cũ đang chạy thì dừng trước khi chạy mới
        if (chaseSoundCoroutine != null)
        {
            bot.StopCoroutine(chaseSoundCoroutine);
        }

        // Bắt đầu Coroutine để phát âm thanh chase (nếu đủ điều kiện)
        chaseSoundCoroutine = bot.StartCoroutine(PlayChaseSoundWithDelay());
    }

    private IEnumerator PlayChaseSoundWithDelay()
    {
        if (audioSource == null) yield break;

        // Đợi ngẫu nhiên từ 1-2 giây trước khi phát chase sound
        yield return new WaitForSeconds(Random.Range(0f, 2f));

        // Kiểm tra nếu zombie vẫn ở trạng thái Chase và không có âm thanh Attack đang phát
        while (audioSource.isPlaying && audioSource.clip == AudioManager.Ins.zombieAttack)
        {
            yield return null; // Chờ cho đến khi attack sound kết thúc
        }

        // Nếu zombie đã chết thì không phát âm thanh nữa
        if (bot.isDed)
        {
            audioSource.enabled = false;
            yield break;
        }

        // Cấu hình âm thanh Chase
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 50f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.volume = 0.2f;
        audioSource.loop = true;

        if (AudioManager.Ins.zombieChase != null)
        {
            audioSource.clip = AudioManager.Ins.zombieChase;
            if (audioSource != null && audioSource.enabled)
            {
                audioSource.Play();
                Debug.Log("Zombie chase sound is playing after delay...");
            }
        }
        else
        {
            Debug.LogError("zombieChase clip is missing in AudioManager!");
        }
    }

    public void OnExecute(Zombie bot)
    {
        if (bot.isDed)
        {
            Debug.Log("A");
            if (audioSource != null)
            {
                Debug.Log("A");
                audioSource.enabled = false; // Tắt âm hoàn toàn nếu zombie chết
            }
            return;
        }

        if (player == null || audioSource == null) return;

        float distance = Vector3.Distance(bot.transform.position, player.position);
        float maxDistance = 100f;

        // Điều chỉnh âm lượng theo khoảng cách
        audioSource.volume = Mathf.Clamp01(1 - (distance / maxDistance));

        if (bot.AbleToAttack())
        {
            bot.TransitionToState(bot.attackState);
            return;
        }

        if (!bot.DetectedPlayer())
        {
            bot.TransitionToState(bot.idleState);
            return;
        }

        bot.ChasePlayer();
    }

    public void OnExit(Zombie bot)
    {
        if (audioSource != null)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }

        bot.navMeshAgent.isStopped = true; // Dừng di chuyển khi rời trạng thái Chase
    }
}
