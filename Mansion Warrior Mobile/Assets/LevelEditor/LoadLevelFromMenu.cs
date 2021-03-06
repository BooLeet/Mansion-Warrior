using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevelFromMenu : MonoBehaviour
{
    public LevelBuilder builder;
    public string levelName;

    void Start()
    {
        if(SceneNameContainer.Instance)
        {
            levelName = SceneNameContainer.Instance.additionalData;
            builder.BuildLevel(LevelLoader.LoadLevel(SceneNameContainer.Instance.additionalData, false), false);
        }
    }
}
