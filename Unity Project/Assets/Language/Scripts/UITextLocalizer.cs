using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextLocalizer : MonoBehaviour
{
    public Text text;
    public string localizationKey;
    private void Start()
    {
        text.text = Localizer.Localize(localizationKey);
    }
}
