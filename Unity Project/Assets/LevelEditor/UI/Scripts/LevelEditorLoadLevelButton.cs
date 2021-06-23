using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorLoadLevelButton : MonoBehaviour
{
    private string levelPath;
    public Text text;

    public void SetLevel(string path)
    {
        levelPath = path;
        text.text = Path.GetFileNameWithoutExtension(path);
    }

    public void LoadLevel()
    {
        LevelEditor.Instance.SetLevelToLoad(levelPath);
    }
}
