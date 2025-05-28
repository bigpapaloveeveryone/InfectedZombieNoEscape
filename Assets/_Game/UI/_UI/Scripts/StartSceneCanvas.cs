using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StartSceneCanvas : UICanvas
{
    [SerializeField] private Button playerBtn;

    [SerializeField] private Button soundBtn;
    [SerializeField] private Sprite[] spr;

    [SerializeField] private AudioSource audioS;

    private bool isClick;

    private void Start()
    {
        playerBtn.onClick.AddListener(() =>
        {
            UIManager.Ins.CloseUI<StartSceneCanvas>();
            GameManager.Ins.StartPlay();
        });

        soundBtn.onClick.AddListener(() =>
        {
            //AudioManager.Ins.PlaySFX(AudioManager.Ins.click);

            isClick = !isClick;
            soundBtn.image.sprite = spr[isClick ? 1 : 0];

            if (isClick)
            {
                audioS.volume = 0f;
            }
            else
            {
                audioS.volume = 0.3f;
            }
        });
    }
}
