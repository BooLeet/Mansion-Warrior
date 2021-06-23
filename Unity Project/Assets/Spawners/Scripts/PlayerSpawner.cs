using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    void Update()
    {
        if(GameManager.Instance.SpawnPlayer())
        {
            Instantiate(playerPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
