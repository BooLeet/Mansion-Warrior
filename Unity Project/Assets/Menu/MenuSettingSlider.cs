using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSettingSlider : MonoBehaviour
{
    public string settingKey;
    public Text textReference;
    public Slider slider;
    public float valueOffset;
    public float valueMultiplier = 1;
    bool started = false;


    void Start()
    {
        started = true;
        Settings.SettingFloatValue setting = Settings.GetFloatValueSetting(settingKey);
        if(setting != null)
            slider.value = (setting.value - valueOffset) / valueMultiplier;
        textReference.text = Localizer.Localize(settingKey);
    }

    public void OnValueChange()
    {
        if(started)
            Settings.ChangeFloatValueSetting(settingKey, valueOffset + slider.value * valueMultiplier);
    }
}
