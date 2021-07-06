using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxCamera : MonoBehaviour
{
    public Camera mainCamera;
    public Camera cam;

    private void Start()
    {
        transform.parent = null;
        transform.position = Vector3.zero;
    }

    void Update()
    {
        transform.rotation = mainCamera.transform.rotation;
        cam.fieldOfView = mainCamera.fieldOfView;
    }
}
