using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSettingCycle : MonoBehaviour
{
    public string settingKey;
    public string[] settingValueKeys;
    [HideInInspector]public int currentValueIndex = 0;
    string value = "";

    public Text textReference;

    private void Start()
    {
        Settings.SettingStringValue setting = Settings.GetStringValueSetting(settingKey);
        if(setting != null)
        {
            value = setting.value;
            for (int i = 0; i < settingValueKeys.Length; ++i)
                if (value == settingValueKeys[i])
                {
                    currentValueIndex = i;
                    break;
                }
        }
        else
            value = settingValueKeys[currentValueIndex];
        UpdateText();
    }

    public void ButtonCallback()
    {
        if (settingValueKeys.Length == 0)
            return;

        currentValueIndex = (currentValueIndex + 1) % settingValueKeys.Length;
        value = settingValueKeys[currentValueIndex];

        Settings.ChangeStringValueSetting(settingKey, value);
        UpdateText();
    }

    private void UpdateText()
    {
        textReference.text = Localizer.Localize(settingKey) + " - " + Localizer.Localize(value);
    }
}
