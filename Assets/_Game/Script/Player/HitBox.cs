using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitBox : MonoBehaviour
{
    public bool isDead;

    [SerializeField] private int HP = 100;
    [SerializeField] private GameObject bloodyScreen;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject indicatorUI;
    [SerializeField] private GameObject menuBtn;

    public void TakeDamage(int damage)
    {
        HP -= damage;

        if (HP <= 0)
        {
            AudioManager.Ins.PlayPlayerClip(AudioManager.Ins.playerDie);
            UIManager.Ins.CloseUI<MainCanvas>();
            isDead = true;
            Debug.Log("Player Dead");
            PlayerDead();

            GetComponentInParent<ScreenFader>().StartFade();
            StartCoroutine(ShowGameOverUI());
        }
        else
        {
            AudioManager.Ins.PlayPlayerClip(AudioManager.Ins.playerHit);
            Debug.Log("Hit");      
            StartCoroutine(BloodyScreenEffect());
        }

        UIManager.Ins.mainCanvas.UpdateHealthBar(HP);
    }

    private IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(1f);
        indicatorUI.SetActive(false);
        gameOverUI.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        menuBtn.SetActive(true);
    }

    private void PlayerDead()
    {
        GetComponentInParent<MouseMovement>().enabled = false;
        GetComponentInParent<PlayerMovement>().enabled = false;

        anim.Play(CacheString.TAG_PDIE);
    }

    private IEnumerator BloodyScreenEffect()
    {
        if (bloodyScreen.activeInHierarchy == false)
        {
            bloodyScreen.SetActive(true);
        }

        var image = bloodyScreen.GetComponentInChildren<Image>();

        // Set the initial alpha value to 1 (fully visible).
        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;

        float duration = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {

            // Calculate the new alpha value using Lerp.
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            // Update the color with the new alpha value.
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;

            // Increment the elapsed time.
            elapsedTime += Time.deltaTime;

            yield return null; ; // Wait for the next frame.
        }

        if (bloodyScreen.activeInHierarchy)
        {
            bloodyScreen.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ZombieHand hand = Cache.GetZombieHand(other);
        if (hand != null)
        {
            if (isDead)
                return;
                
            TakeDamage(hand.damage);
        }
    }
}
