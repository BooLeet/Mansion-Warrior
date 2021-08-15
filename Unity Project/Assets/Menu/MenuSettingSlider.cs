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
    private float settingValue = 0;

    void Start()
    {
        started = true;
        Settings.SettingFloatValue setting = Settings.GetFloatValueSetting(settingKey);
        if(setting != null)
        {
            settingValue = setting.value;
            slider.value = (setting.value - valueOffset) / valueMultiplier;
        }
        textReference.text = Localizer.Localize(settingKey) + " (" + settingValue + ")";
    }

    public void OnValueChange()
    {
        if (!started)
            return;
        settingValue = valueOffset + slider.value * valueMultiplier;
        Settings.ChangeFloatValueSetting(settingKey, settingValue);
        textReference.text = Localizer.Localize(settingKey) + " (" + settingValue + ")";
    }
}
