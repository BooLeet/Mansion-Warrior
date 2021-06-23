using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode : MonoBehaviour
{
    public static GameMode Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public abstract bool SpawnPlayer();

    public abstract bool SpawnEnemy(Vector3 position);

    public abstract void FailEnd();

    public abstract void SuccessEnd();

    public abstract void OnEnemyDamaged();
}
