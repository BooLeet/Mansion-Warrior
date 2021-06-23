using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Ammo : MonoBehaviour
{
    public Text allAmmoCount, loadedAmmoCount;

    public void SetValues(uint allAmmo, uint loadedAmmo)
    {
        allAmmoCount.text = allAmmo.ToString();
        loadedAmmoCount.text = loadedAmmo.ToString();
    }
}
