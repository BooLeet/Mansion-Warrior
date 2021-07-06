using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetWarmup : MonoBehaviour
{
    public GameObject[] objectsToWarmup;

    void Start()
    {
        foreach(GameObject obj in objectsToWarmup)
        {
            DestroyImmediate(Instantiate(obj));
        }
    }

}
