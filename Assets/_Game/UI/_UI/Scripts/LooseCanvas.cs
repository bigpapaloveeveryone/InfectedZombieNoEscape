using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LooseCanvas : UICanvas
{
/*    [Header("---Other Button---")]
    [SerializeField] private Button retryBtn;
    [SerializeField] private Button menuBtn;
    //[SerializeField] private Button retryBtn;

    [Header("---Music Button---")]
    [SerializeField] private Button soundBtn;
    [SerializeField] private Sprite[] spr;

    private bool isClick;

    private void OnEnable()
    {
        AudioManager.Ins.PlaySFX(AudioManager.Ins.loose);
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }


    private void Start()
    {
        retryBtn.onClick.AddListener(() =>
        {
            AudioManager.Ins.PlaySFX(AudioManager.Ins.click);
            UIManager.Ins.CloseUI<LooseCanvas>();
            UIManager.Ins.OpenUI<MainCanvas>();
            LevelManager.Ins.LoadMapByID(LevelManager.Ins.curMapID);
        });

        menuBtn.onClick.AddListener(() =>
        {
            AudioManager.Ins.PlaySFX(AudioManager.Ins.click);

            LevelManager.Ins.DespawnMap();
            UIManager.Ins.CloseUI<LooseCanvas>();
            UIManager.Ins.OpenUI<ChooseLevelCanvas>();
        });

        *//*retryBtn.onClick.AddListener(() =>
        {
            AudioManager.Ins.PlaySFX(AudioManager.Ins.click);
            UIManager.Ins.CloseUI<WinCanvas>();
            UIManager.Ins.OpenUI<MainCanvas>();
            LevelManager.Ins.LoadMapByID(LevelManager.Ins.curMapID);
        });*//*

        soundBtn.onClick.AddListener(() =>
        {
            AudioManager.Ins.PlaySFX(AudioManager.Ins.click);
            isClick = !isClick;
            soundBtn.image.sprite = spr[isClick ? 1 : 0];

            if (isClick)
            {
                AudioManager.Ins.TurnOff();
            }
            else
            {
                AudioManager.Ins.TurnOn();
            }
        });
    }*/
}
