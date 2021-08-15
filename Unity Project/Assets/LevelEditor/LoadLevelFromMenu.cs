using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevelFromMenu : MonoBehaviour
{
    public LevelBuilder builder;
    private string levelName;

    void Start()
    {
        if(SceneNameContainer.Instance)
        {
            levelName = SceneNameContainer.Instance.additionalData;
            builder.BuildLevel(LevelLoader.LoadLevel(levelName, false), false);
        }
    }

    public string GetLevelName()
    {
        return levelName;
    }
}
