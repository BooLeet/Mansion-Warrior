using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelEditor/Object Set")]
public class LevelEditorObjectSet : ScriptableObject
{
    public string setName;
    public GameObject[] objects;
}
