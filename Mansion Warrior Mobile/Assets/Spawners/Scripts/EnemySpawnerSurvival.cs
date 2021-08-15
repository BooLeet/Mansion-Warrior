using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerSurvival : MonoBehaviour
{
    private GameModeSurvival gameMode;
    private float cooldownTimeCounter = 0;
    public GameObject spawnEffect;
    //bool isValid = false;

    private void Start()
    {
        gameMode = GameMode.Instance as GameModeSurvival;
    }

    void LateUpdate()
    {
        if (gameMode == null)
        {
            gameMode = GameMode.Instance as GameModeSurvival;
            return;
        }

        if (cooldownTimeCounter <= 0)
        {
            GameObject enemyPrefab = gameMode.GetEnemyToSpawn(gameObject, out float cooldown);

            cooldownTimeCounter = cooldown;
            if (enemyPrefab)
            {
                Instantiate(enemyPrefab, transform.position, transform.rotation);
                if (spawnEffect)
                    Instantiate(spawnEffect, transform.position + Vector3.up, Quaternion.identity);
            }
                
        }

        cooldownTimeCounter -= Time.deltaTime;
    }

}
