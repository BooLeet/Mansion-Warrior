using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionLightEffect : MonoBehaviour
{
    public float duration = 5;
    public Light lightSource;
    float startingIntensity;

    private void Start()
    {
        startingIntensity = lightSource.intensity;
    }

    void Update()
    {
        if (lightSource.intensity <= 0)
            return;

        lightSource.intensity -= Time.deltaTime * startingIntensity / duration;
    }
}
