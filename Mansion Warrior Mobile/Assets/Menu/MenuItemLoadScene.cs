using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemLoadScene : MonoBehaviour
{
    public string sceneName;
    [Space]
    public Text textReference;
    public string localizationKey;

    void Start()
    {
        textReference.text = Localizer.Localize(localizationKey);
    }

    public void ButtonCallback()
    {
        SceneNameContainer.Instance.sceneName = sceneName;
        SceneNameContainer.Instance.BeginLoading();
    }
}
