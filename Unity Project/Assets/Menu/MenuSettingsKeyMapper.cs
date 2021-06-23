using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSettingsKeyMapper : MonoBehaviour
{
    public Transform contentTransform;
    public GameObject keyButtonPrefab;
    public GameObject resetKeysButtonPrefab;
    public GameObject backButtonPrefab;
    [Space]
    public MenuSet keyMapWindow;
    private MenuSettingKey keyToChange;

    private void Start()
    {
        FillButtons();
    }

    public void FillButtons()
    {
        Utility.RemoveChildren(contentTransform);
        List<Settings.SettingKeyCodeValue> keys = Settings.GetKeyCodeList();
        foreach (var key in keys)
        {
            Instantiate(keyButtonPrefab, contentTransform).GetComponent<MenuSettingKey>().Setup(key.key, key.value, this);
        }
        Instantiate(resetKeysButtonPrefab, contentTransform).GetComponent<MenuSettingKeyReset>().Setup(this);
        Instantiate(backButtonPrefab, contentTransform);
    }

    public void OpenKeyMapWindow(MenuSettingKey menuSettingKey)
    {
        keyToChange = menuSettingKey;
        Menu.NextSet(keyMapWindow);
    }

    public void BindKey(KeyCode key)
    {
        keyToChange.ChangeValue(key);
        Menu.GoToPreviousSet();
    }
}
