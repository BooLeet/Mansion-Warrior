using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSet : MonoBehaviour
{
    public string setNameKey;

    private void Start()
    {
        Menu.RegisterSet(this);
    }
}
