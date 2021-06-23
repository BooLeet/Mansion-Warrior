using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSettingResetAll : MonoBehaviour
{
    public Text textReference;
    public string localizationKey;

    private void Start()
    {
        textReference.text = Localizer.Localize(localizationKey);
    }

    public void ButtonCallback()
    {
        SettingsLoader.SaveSettings(new Settings());
        Settings.GetSettings().ApplyAllSettings();
    }
}
