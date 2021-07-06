using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelectorButton : MonoBehaviour
{
    private Level.Index itemIndex;
    private LevelEditor.SelectedItem.ItemType itemType;

    public void SetIndex(Level.Index index, LevelEditor.SelectedItem.ItemType type)
    {
        itemIndex = index;
        itemType = type;
    }

    public void ButtonFunction()
    {
        LevelEditor.Instance.ChangeSelectedItem(new LevelEditor.SelectedItem(itemIndex,itemType));
    }
}
