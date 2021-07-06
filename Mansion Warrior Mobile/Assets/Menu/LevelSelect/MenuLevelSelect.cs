using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLevelSelect : MonoBehaviour
{
    public RectTransform levelSelectorContent;
    public GameObject levelButtonPrefab;
    public GameObject backButtonPrefab;

    void Start()
    {
        LoadScreenUpdateLevels(Application.dataPath);
    }

    public void LoadScreenUpdateLevels(string path)
    {
        Utility.RemoveChildren(levelSelectorContent);
        string[] files = System.IO.Directory.GetFiles(path, "*.level");
        foreach (string file in files)
        {
            GameObject obj = Instantiate(levelButtonPrefab, levelSelectorContent);
            MenuLevelSelectButton loadLevelButton = obj.GetComponent<MenuLevelSelectButton>();
            loadLevelButton.SetLevel(file);
        }
        Instantiate(backButtonPrefab, levelSelectorContent);
    }
}
