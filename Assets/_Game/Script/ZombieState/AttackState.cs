using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState<Zombie>
{
    public void OnEnter(Zombie bot)
    {
        bot.navMeshAgent.isStopped = true;
        bot.navMeshAgent.velocity = Vector3.zero;
        bot.PlayAnim(CacheString.TAG_ZATTACK);

        // Nếu zombie có AudioSource, phát âm thanh tấn công
        if (bot.audioSource != null && AudioManager.Ins.zombieAttack != null)
        {
            bot.audioSource.PlayOneShot(AudioManager.Ins.zombieAttack);
        }
    }

    public void OnExecute(Zombie bot)
    {
        if (bot.isDed)
            return;

        if (!bot.AbleToAttack())
        {
            if (bot.DetectedPlayer())
            {
                bot.TransitionToState(bot.chaseState);
            }
        }

        bot.Attack();
    }

    public void OnExit(Zombie bot)
    {
        // Dừng ngay lập tức âm thanh của zombie khi chuyển state
        if (bot.audioSource != null)
        {
            bot.audioSource.Stop();
        }
    }
}
