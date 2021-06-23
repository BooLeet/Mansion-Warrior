using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "LevelEditor/Collection")]
public class LevelEditorCollection : ScriptableObject
{
    public LevelLighting[] levelLightings;
    public LevelEditorMaterialSet[] materials;
    public LevelEditorObjectSet[] objects;
    public GameObject errorObject;


    public Material GetMaterial(Level.Index index)
    {
        return GetMaterial(index.setName, index.setIndex);
    }

    public Material GetMaterial(string setName, string index)
    {
        var materialSets = from set in materials
                          where set.setName == setName
                          select set;

        if (materialSets.Count() == 0)
            return null;
        var foundMaterials = from material in (materialSets.First().materials)
                            where material.name == index
                            select material;

        if (foundMaterials.Count() == 0)
            return null;
        return foundMaterials.First();
    }

    public GameObject GetObject(Level.Index index)
    {
        return GetObject(index.setName, index.setIndex);
    }

    public GameObject GetObject(string setName, string index)
    {
        var objectSets = from set in objects
                          where set.setName == setName
                          select set;

        if (objectSets.Count() == 0)
            return errorObject;
        var foundObjects = from obj in (objectSets.First().objects)
                           where obj.name == index
                           select obj;

        if (foundObjects.Count() == 0)
            return errorObject;

        return foundObjects.First();
    }

    public LevelLighting GetLevelLighting(string name)
    {
        var foundObjects = from lighting in levelLightings
                           where lighting.name == name
                           select lighting;
        if (foundObjects.Count() == 0)
            return null;
        return foundObjects.First();
    }
}
