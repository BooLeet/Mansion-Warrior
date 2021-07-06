using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSettingKey : MonoBehaviour
{
    public Text text;
    private string key;
    private KeyCode val;
    private MenuSettingsKeyMapper keyMapper;


    public void Setup(string key, KeyCode val,MenuSettingsKeyMapper keyMapper)
    {
        this.key = key;
        this.val = val;
        this.keyMapper = keyMapper;
        UpdateText();
    }

    public void ButtonCallback()
    {
        keyMapper.OpenKeyMapWindow(this);
    }

    public void ChangeValue(KeyCode newVal)
    {
        val = newVal;
        Settings.ChangeKeyCodeValueSetting(key, val);
        UpdateText();
    }

    public void UpdateText()
    {
        text.text = Localizer.Localize(key) + " - " + Localizer.LocalizeKeyCode(val);
    }
}
