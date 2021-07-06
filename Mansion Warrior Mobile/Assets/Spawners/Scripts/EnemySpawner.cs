using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    private bool spawnerActive = true;

    private void Start()
    {
        GameManager.Instance.RegisterEnemySpawner(this);
    }

    void Update()
    {
        if (spawnerActive && GameManager.Instance.SpawnEnemy(transform.position))
        {
            Instantiate(enemyPrefab, transform.position, transform.rotation);
            spawnerActive = false;
        }
    }

    public void Activate()
    {
        spawnerActive = true;
    }
}
