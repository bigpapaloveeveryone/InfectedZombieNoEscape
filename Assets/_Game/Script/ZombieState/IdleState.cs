using System.Collections;
using UnityEngine;

public class IdleState : IState<Zombie>
{
    private AudioSource audioSource;
    private Transform player;
    private Zombie bot;
    private Coroutine idleSoundCoroutine;

    public void OnEnter(Zombie bot)
    {
        this.bot = bot;

        bot.enabled = true;
        bot.navMeshAgent.isStopped = true;
        bot.navMeshAgent.velocity = Vector3.zero;
        bot.PlayAnim(CacheString.TAG_ZIDLE);

        audioSource = bot.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("Zombie is missing an AudioSource!");
            return;
        }

        player = GameManager.Ins.player.transform;

        // Nếu có coroutine cũ đang chạy thì dừng nó trước khi chạy mới
        if (idleSoundCoroutine != null)
        {
            bot.StopCoroutine(idleSoundCoroutine);
        }

        // Bắt đầu Coroutine để phát âm thanh idle (nếu đủ điều kiện)
        idleSoundCoroutine = bot.StartCoroutine(PlayIdleSoundWithDelay());
    }

    private IEnumerator PlayIdleSoundWithDelay()
    {
        if (audioSource == null) yield break;

        // Đợi ngẫu nhiên từ 1-2 giây trước khi phát idle sound
        yield return new WaitForSeconds(Random.Range(1f, 8f));

        // Kiểm tra nếu zombie vẫn ở trạng thái Idle và không có âm thanh Attack đang phát
        while (audioSource.isPlaying && audioSource.clip == AudioManager.Ins.zombieAttack)
        {
            yield return null; // Chờ cho đến khi attack sound kết thúc
        }

        // Nếu zombie đã chết thì không phát âm thanh nữa
        if (bot.isDed) yield break;

        // Cấu hình âm thanh Idle
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 50f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.volume = 0.2f;
        audioSource.loop = true;

        if (AudioManager.Ins.zombieWalking != null)
        {
            audioSource.clip = AudioManager.Ins.zombieWalking;

            if (audioSource != null && audioSource.enabled)
            {
                audioSource.Play();
                Debug.Log("Zombie idle sound is playing after delay...");
            }
        }
        else
        {
            Debug.LogError("zombieIdle clip is missing in AudioManager!");
        }
    }

    public void OnExecute(Zombie bot)
    {
        if (bot.isDed || player == null || audioSource == null) return;

        float distance = Vector3.Distance(bot.transform.position, player.position);
        float maxDistance = 100f;

        // Điều chỉnh âm lượng theo khoảng cách
        audioSource.volume = Mathf.Clamp01(1 - (distance / maxDistance));

        if (bot.AbleToAttack())
        {
            bot.TransitionToState(bot.attackState);
            return;
        }

        if (bot.DetectedPlayer())
        {
            bot.TransitionToState(bot.chaseState);
            return;
        }
    }

    public void OnExit(Zombie bot)
    {
        if (audioSource != null)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }

        bot.navMeshAgent.isStopped = false;
    }
}
