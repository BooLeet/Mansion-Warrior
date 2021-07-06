using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool ListenForPause { get; set; }
    private bool isPaused = false;
    public Settings Settings { get; private set; }
    private List<EnemySpawner> enemySpawners = new List<EnemySpawner>();
    private List<ResourcePickup> resourcePickups = new List<ResourcePickup>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        ListenForPause = true;
        Unpause();
        Settings = SettingsLoader.LoadSettings();
        Application.targetFrameRate = 1000;
    }

    #region Pause
    public static bool IsPaused()
    {
        if (Instance == null)
            return false;
        return Instance.isPaused;
    }

    public static void Pause()
    {
        if (Instance == null)
            return;
        Instance.isPaused = true;
        Utility.Pause();
    }

    public static void Unpause()
    {
        if (Instance == null)
            return;
        Instance.isPaused = false;
        Utility.Unpause();
    }
    #endregion

    #region Spawning
    public bool SpawnPlayer()
    {
        if (GameMode.Instance)
            return GameMode.Instance.SpawnPlayer();
        return false;
    }

    public bool SpawnEnemy(Vector3 position)
    {
        if (GameMode.Instance)
            return GameMode.Instance.SpawnEnemy(position);
        return false;
    }


    public void RegisterEnemySpawner(EnemySpawner spawner)
    {
        enemySpawners.Add(spawner);
    }

    public void ActivateAllEnemySpawners()
    {
        foreach (EnemySpawner spawner in enemySpawners)
            spawner.Activate();
    }
    #endregion

    #region Pickups

    public void RegisterPickup(ResourcePickup pickup)
    {
        resourcePickups.Add(pickup);
    }

    public void UnregisterPickup(ResourcePickup pickup)
    {
        resourcePickups.Remove(pickup);
    }

    public void ResetPickups()
    {
        for (int i = 0; i < resourcePickups.Count; ++i)
        {
            bool destroyed = resourcePickups[i].DestroyOnPickup;
            resourcePickups[i].ResetPickup();
            if (destroyed)
                --i;
        }
    }

    #endregion
}
