using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("----- Audio Source -----")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    
    public AudioSource playerSource;
    public AudioSource zombieSource;

    [Header("----- Audio Clip -----")]
    public AudioClip backgroundClip;
    public AudioClip click;
    public AudioClip win;
    public AudioClip loose;
    public AudioClip collect;
    public AudioClip shoot;
    public AudioClip reload;
    public AudioClip outOfAmmo;

    public AudioClip zombieWalking;
    public AudioClip zombieChase;
    public AudioClip zombieAttack;
    public AudioClip zombieHurt;
    public AudioClip zombieDeath;

    public AudioClip playerHit;
    public AudioClip[] playerWalk;
    public AudioClip playerDie;

    /*private void Start()
    {
        musicSource.clip = backgroundClip;
        musicSource.Play();
    }*/

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayZombieClip(AudioClip clip)
    {
        zombieSource.PlayOneShot(clip);
    }

    public void PlayPlayerClip(AudioClip clip)
    {
        playerSource.PlayOneShot(clip);
    }

    public void TurnOff()
    {
        //this.enabled = false;
        sfxSource.volume = 0f;
        musicSource.volume = 0f;
    }

    public void TurnOn()
    {
        //this.enabled = true;
        sfxSource.volume = 1f;
        musicSource.volume = 0.5f;
    }
}
