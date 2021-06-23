using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelEditor/Material Set")]
public class LevelEditorMaterialSet : ScriptableObject
{
    public string setName;
    public Material[] materials;
}
