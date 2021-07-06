using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFXOnStart : MonoBehaviour
{
    public AudioClip clip;
    public float spatialBlend = 1;
    void Start()
    {
        Audio.PlaySFX(clip, transform.position, transform, spatialBlend);
    }
}
