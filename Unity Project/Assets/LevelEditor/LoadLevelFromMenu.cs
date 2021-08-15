using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevelFromMenu : MonoBehaviour
{
    public LevelBuilder builder;
    private string levelName;
    public string menuSceneName = "Menu";

    void Start()
    {
        if(SceneNameContainer.Instance)
        {
            levelName = SceneNameContainer.Instance.additionalData;
            Level level = LevelLoader.LoadLevel(levelName, false);
            if(level != null)
                builder.BuildLevel(level, false);
            else
                SceneNameContainer.Instance.LoadScene(menuSceneName);
        }
    }

    public string GetLevelName()
    {
        return levelName;
    }
}
