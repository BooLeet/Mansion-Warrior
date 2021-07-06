using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLaser : MonoBehaviour
{
    public float laserRange = 50;
    public Transform laserSource;
    public GameObject laserDot;

    void Update()
    {
        Ray ray = new Ray(laserSource.position, laserSource.forward);
        RaycastHit raycastHit;

        if(Physics.Raycast(ray,out raycastHit,laserRange))
        {
            laserDot.SetActive(true);
            laserDot.transform.position = raycastHit.point + raycastHit.normal * 0.025f;
        }
        else
            laserDot.SetActive(false);
    }
}
