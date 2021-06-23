using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorCancelLoad : MonoBehaviour
{
    public void CancelLoad()
    {
        LevelEditor.Instance.CancelLoad = true;
    }
}
