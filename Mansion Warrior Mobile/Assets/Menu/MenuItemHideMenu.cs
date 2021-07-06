using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemHideMenu : MonoBehaviour
{
    public Text textReference;
    public string localizationKey = "menuResume";

    private void Start()
    {
        textReference.text = Localizer.Localize(localizationKey);
    }

    public void ButtonCallback()
    {
        Menu.Hide();
    }
}
