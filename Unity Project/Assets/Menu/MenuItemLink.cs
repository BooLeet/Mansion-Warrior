using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemLink : MonoBehaviour
{
    public MenuSet link;
    public Text textReference;

    private void Start()
    {
        if(link)
            textReference.text = Localizer.Localize(link.setNameKey);
    }

    public void ButtonCallback()
    {
        Menu.NextSet(link);
    }
}
