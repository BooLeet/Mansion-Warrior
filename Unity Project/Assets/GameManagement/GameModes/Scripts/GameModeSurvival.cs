using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeSurvival : GameMode
{
    public AIDirector AIDirector;
    public GameObject regularEnemyPrefab;
    public GameObject mediumEnemyPrefab;
    public GameObject hardEnemyPrefab;

    private PlayerCharacter player;

    public float CurrentSpawnCooldown { get; private set; }
    

    
    public LayerMask enemySpawnCheckMask;

    private SurvivalFSM.State currentState;

    private bool counterStarted = false;
    [Header("Stats")]
    public float pauseBeforeStart = 2;
    public float enemyCountTimeMultiplier = 2;
    public float enemyCountLogMultiplier = 10;
    public float enemyCountOffset = 5;
    [Space]
    public float mediumEnemySpawnDelay = 60;
    [Range(0,1/3f)]
    public float mediumEnemySpawnProbability = 0.1f;
    [Space]
    public float hardEnemySpawnDelay = 120;
    [Range(0, 1 / 3f)]
    public float hardEnemySpawnProbability = 0.05f;
    [Space]
    public float enemySpawnStartDistance = 8;
    public float enemySpawnEndDistance = 50;

    // Counters
    public float TimeCounter { get; private set; }
    public int PTS { get; private set; }
    public float PTSMultiplier { get; private set; }

    private void Start()
    {
        player = PlayerCharacter.Instance;
        CurrentSpawnCooldown = 2;
        currentState = new SurvivalFSM.SurvivalPauseBeforeWave();
        currentState.Init(this);
    }

    private void Update()
    {
        if (player == null)
            player = PlayerCharacter.Instance;

        if (counterStarted)
            TimeCounter += Time.deltaTime;

        currentState.Action(this);
        SurvivalFSM.State nextState = currentState.Transition(this);
        if (nextState != null)
        {
            currentState = nextState;
            currentState.Init(this);
        }
    }

    public override bool SpawnEnemy(Vector3 position)
    {
        return false;
    }

    public void StartCounter()
    {
        counterStarted = true;
    }

    public GameObject GetEnemyToSpawn(Vector3 position, out float cooldown)
    {
        cooldown = 0;
        if (player == null || !counterStarted)
            return null;
        if (AIDirector.GetAICount() >= GetCurrentEnemyCount())
            return null;

        float distance = Vector3.Distance(position, PlayerCharacter.Instance.Position);
        if (distance > enemySpawnEndDistance || distance < enemySpawnStartDistance)
            return null;

        if (Utility.WithinAngle(player.head.position, player.head.forward, position, 90) && Utility.IsVisible(position + Vector3.up, PlayerCharacter.Instance.gameObject, enemySpawnEndDistance, PlayerCharacter.Instance.verticalTargetingOffset, enemySpawnCheckMask))
            return null;

        cooldown = CurrentSpawnCooldown;
        GameObject objToSpawn = regularEnemyPrefab;
        if (TimeCounter >= mediumEnemySpawnDelay)
            objToSpawn = Random.value < mediumEnemySpawnProbability ? mediumEnemyPrefab : objToSpawn;

        if (TimeCounter >= hardEnemySpawnDelay)
            objToSpawn = Random.value < hardEnemySpawnProbability ? hardEnemyPrefab : objToSpawn;

        return objToSpawn;
    }

    public override bool SpawnPlayer()
    {
        return true;
    }

    public override void FailEnd()
    {
        SceneNameContainer.Instance.BeginLoading();
    }

    public override void SuccessEnd()
    {

    }

    public int GetCurrentEnemyCount()
    {
        return (int)(enemyCountLogMultiplier * System.Math.Log(1 + enemyCountTimeMultiplier * TimeCounter / 60) + enemyCountOffset);
    }

    public override void OnEnemyDamaged()
    {
        
    }
}

namespace SurvivalFSM
{
    abstract class State
    {
        public abstract void Init(GameModeSurvival gameMode);
        public abstract void Action(GameModeSurvival gameMode);
        public abstract State Transition(GameModeSurvival gameMode);
    }

    class SurvivalPauseBeforeWave : State
    {
        float timeCounter = 0;

        public override void Action(GameModeSurvival gameMode)
        {
            timeCounter -= Time.deltaTime;
        }

        public override void Init(GameModeSurvival gameMode)
        {
            timeCounter = gameMode.pauseBeforeStart;
        }

        public override State Transition(GameModeSurvival gameMode)
        {
            if (timeCounter <= 0)
                return new SurvivalWave();
            return null;
        }
    }

    class SurvivalWave : State
    {
        public override void Action(GameModeSurvival gameMode)
        {
            
        }

        public override void Init(GameModeSurvival gameMode)
        {
            //gameMode.CurrentWaveEnemyCount = gameMode.GetCurrentEnemyCount(++gameMode.CurrentWave);
            //Debug.Log(gameMode.CurrentWave);
            gameMode.StartCounter();
        }

        public override State Transition(GameModeSurvival gameMode)
        {
            //if (gameMode.CurrentWaveEnemyCount > 0)
            //    return null;
            //return new SurvivalPauseBeforeWave();
            return null;
        }
    }
}

