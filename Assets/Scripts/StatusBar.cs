using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public Text healthText;
    public Text ammoText;
    public Image healthBar;
    public Image ammoBar;

    public void updateBar(int health, int ammo, int maxAmmo, bool reloading)
    {
        healthText.text = health.ToString();
        ammoText.text = (reloading) ? "RELOADING" : ammo.ToString();
 
        healthBar.fillAmount = health / 100f;
        if (ammo <= maxAmmo)
        {
            ammoBar.fillAmount = (float)ammo / maxAmmo;
        }

    }
}
