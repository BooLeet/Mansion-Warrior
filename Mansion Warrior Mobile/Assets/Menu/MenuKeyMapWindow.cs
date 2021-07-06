using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuKeyMapWindow : MonoBehaviour
{
    public MenuSettingsKeyMapper keyMapper;
    private KeyCode keyCode;

    void Update()
    {
        List<KeyCode> pressedKeys = Utility.GetPressedKeycodes();
        if (pressedKeys.Count == 0)
            return;

        keyMapper.BindKey(pressedKeys[0]);
    }
}
