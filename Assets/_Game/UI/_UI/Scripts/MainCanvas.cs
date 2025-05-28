using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : UICanvas
{
    [SerializeField] private Text ammoTxt;
    [SerializeField] private Image healthBar;
    [SerializeField] private Text curWave;

    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private Button shootBtn;
    [SerializeField] private Button reloadBtn;

    [SerializeField] private Sprite[] reloadSpr;

    private void Awake()
    {
        UIManager.Ins.mainCanvas = this;
    }

    public FixedJoystick GetJoyStick()
    {
        return joystick;
    }

    public Button GetShootButton()
    {
        return shootBtn;
    }

    public Button GetReloadButton()
    {
        return reloadBtn;
    }


    public void TextAmmo(int bulletsLeft, int bulletsPerBurst, int magazineSize)
    {
        ammoTxt.text = $"{bulletsLeft / bulletsPerBurst}/{magazineSize / bulletsPerBurst}";
    }

    public void UpdateHealthBar(int currentHealth)
    {
        int maxHealth = 100;
        healthBar.fillAmount = Mathf.Clamp01((float)currentHealth / maxHealth);
    }

    public void UpdateCurWave(int wave)
    {
        curWave.text = "Wave: " + wave.ToString();
    }

    public void IsReloading(bool done)
    {
        shootBtn.interactable = !done;
        reloadBtn.image.sprite = done ? reloadSpr[1] : reloadSpr[0];
        reloadBtn.interactable = !done;
    }
}
