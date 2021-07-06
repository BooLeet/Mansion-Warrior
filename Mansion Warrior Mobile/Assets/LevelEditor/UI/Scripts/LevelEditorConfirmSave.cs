using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorConfirmSave : MonoBehaviour
{
    public void ConfirmSave()
    {
        LevelEditor.Instance.ConfirmSave = true;
    }

    public void CancelSave()
    {
        LevelEditor.Instance.CancelSave = true;
    }
}
