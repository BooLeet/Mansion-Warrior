using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSettingKeyReset : MonoBehaviour
{
    public Text textReference;
    public string localizationKey;
    private MenuSettingsKeyMapper mapper;

    private void Start()
    {
        textReference.text = Localizer.Localize(localizationKey);
    }

    public void Setup(MenuSettingsKeyMapper mapper)
    {
        this.mapper = mapper;
    }

    public void ButtonCallback()
    {
        Settings.ResetKeys();
        mapper.FillButtons();
        SettingsLoader.SaveSettings(Settings.GetSettings());
    }
}
