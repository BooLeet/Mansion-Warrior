using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelectorButton : MonoBehaviour
{
    private Level.Index objectIndex;

    public void SetIndex(Level.Index index)
    {
        objectIndex = index;
    }

    public void ButtonFunction()
    {
        //LevelEditor.Instance.ChangeSelectedObject(objectIndex);
    }
}
