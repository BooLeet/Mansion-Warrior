using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_KeyCode : MonoBehaviour
{
    public Text text;
    public string keyName;
    public bool showName = false;

    void Update()
    {
        text.text = (showName? Localizer.Localize(keyName) + " " : "") + Localizer.LocalizeKeyCode(Settings.GetKeyCode(keyName));
    }
}
