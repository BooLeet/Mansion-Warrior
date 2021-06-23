using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTestLevel : MonoBehaviour
{
    public LevelBuilder builder;
    public string levelName = "testLevel";

    void Start()
    {
        builder.BuildLevel(LevelLoader.LoadLevel(levelName, false), false);
    }
}
