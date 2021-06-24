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
        //isValid = false;
        if (cooldownTimeCounter <= 0)
        {
            GameObject enemyPrefab = gameMode.GetEnemyToSpawn(gameObject, out float cooldown);

            //isValid = enemyPrefab != null;
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

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    if (isValid)
    //        Gizmos.color = Color.green;
    //    Gizmos.DrawCube(transform.position, Vector3.one * 2);
    //}
}
