using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(LoadTestLevel))]
public class LoadTestLevelEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LoadTestLevel obj = (LoadTestLevel)target;
        if (GUILayout.Button("Build"))
        {
            obj.builder.BuildLevel(LevelLoader.LoadLevel(obj.levelName, false), false);
            EditorUtility.SetDirty(obj.gameObject);
        }

        if (GUILayout.Button("Clear"))
        {
            obj.builder.ClearLevel();
            EditorUtility.SetDirty(obj.gameObject);
        }
    }
}
