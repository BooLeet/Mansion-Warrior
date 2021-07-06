using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera : MonoBehaviour
{
    public Camera thisCamera;
    public Camera mainCamera;

    void Update()
    {
        thisCamera.fieldOfView = mainCamera.fieldOfView;
    }
}
