using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemQuit : MonoBehaviour
{
    public Text textReference;
    public string localizationKey;

    void Start()
    {
        textReference.text = Localizer.Localize(localizationKey);
    }

    public void ButtonCallback()
    {
        Application.Quit();
    }
}
