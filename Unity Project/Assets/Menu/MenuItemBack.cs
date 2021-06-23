using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemBack : MonoBehaviour
{
    public Text textReference;
    public string localizationKey = "menuBack";

    private void Start()
    {
        textReference.text = Localizer.Localize(localizationKey);
    }

    public void ButtonCallback()
    {
        Menu.GoToPreviousSet();
    }

}
