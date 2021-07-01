using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_ButtonHelper : MonoBehaviour
{
    public string settingKey = "settingButtonHelper";
    public GameObject objToSwitch;

    void Update()
    {
        objToSwitch.SetActive(Settings.GetStringValueSetting(settingKey).value == "on");
    }
}
