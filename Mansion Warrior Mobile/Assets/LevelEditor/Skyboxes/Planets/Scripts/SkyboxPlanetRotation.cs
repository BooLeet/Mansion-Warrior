using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxPlanetRotation : MonoBehaviour
{
    public float speed = 0.5f;

    void Update()
    {
        transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0));
    }
}
