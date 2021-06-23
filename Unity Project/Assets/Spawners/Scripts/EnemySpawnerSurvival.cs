using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerSurvival : MonoBehaviour
{
    private GameModeSurvival gameMode;
    private float cooldownTimeCounter = 0;

    private void Start()
    {
        gameMode = GameMode.Instance as GameModeSurvival;
    }

    void Update()
    {
        if (gameMode == null)
        {
            gameMode = GameMode.Instance as GameModeSurvival;
            return;
        }

        if (cooldownTimeCounter <= 0)
        {
            GameObject enemyPrefab = gameMode.GetEnemyToSpawn(transform.position, out float cooldown);
            cooldownTimeCounter = cooldown;
            if (enemyPrefab)
                Instantiate(enemyPrefab, transform.position, transform.rotation);
        }

        cooldownTimeCounter -= Time.deltaTime;
    }
}
