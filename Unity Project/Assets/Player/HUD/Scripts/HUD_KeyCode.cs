using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_KeyCode : MonoBehaviour
{
    public Text text;
    public string keyName;

    void Update()
    {
        text.text = Localizer.LocalizeKeyCode(Settings.GetKeyCode(keyName));
    }
}
