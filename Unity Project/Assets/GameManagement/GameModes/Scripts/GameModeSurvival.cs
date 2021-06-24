using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeSurvival : GameMode
{
    public GameObject gameOverMenu;
    public Text gameOverText;
    public GameObject hudElement;
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
    public float enemySpawnvisibilityCheckAngle = 100;

    [Header("PTS")]
    public int PTSMultiplierMaxStage = 5;
    public float PTSMultiplierDecayDelay = 3;
    public float PTSMultiplierDecaySpeed = 25;
    public float PTSMultiplierPerStage = 500;

    public int PTSMultiplier { get { return (int)(Mathf.Pow(2, Mathf.Clamp((int)(PTSMultiplierMeter / PTSMultiplierPerStage), 0, PTSMultiplierMaxStage))); } }
    public float PTSMultiplierMeter { get; private set; }
    private float PTSMultiplierDecayTimer = 0;

    // Counters
    public float TimeCounter { get; private set; }
    public int PTS { get; private set; }
    

    private void Start()
    {
        gameOverMenu.SetActive(false);
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

        PTSMultiplierUpdate();
    }

    public override bool SpawnEnemy(Vector3 position)
    {
        return false;
    }

    public void StartCounter()
    {
        counterStarted = true;
    }

    public GameObject GetEnemyToSpawn(GameObject spawner, out float cooldown)
    {
        cooldown = 0;
        if (player == null || !counterStarted)
            return null;
        if (AIDirector.GetAICount() >= GetCurrentEnemyCount())
            return null;

        if (!SpawnerIsValid(spawner))
            return null;

        cooldown = CurrentSpawnCooldown;
        GameObject objToSpawn = regularEnemyPrefab;
        if (TimeCounter >= mediumEnemySpawnDelay)
            objToSpawn = Random.value < mediumEnemySpawnProbability ? mediumEnemyPrefab : objToSpawn;

        if (TimeCounter >= hardEnemySpawnDelay)
            objToSpawn = Random.value < hardEnemySpawnProbability ? hardEnemyPrefab : objToSpawn;

        return objToSpawn;
    }

    public bool SpawnerIsValid(GameObject spawner)
    {
        if (player == null)
            return false;

        Vector3 position = spawner.transform.position;
        float distance = Vector3.Distance(position, player.head.position);
        if (enemySpawnEndDistance < distance || distance < enemySpawnStartDistance)
            return false;
        float angle = enemySpawnvisibilityCheckAngle * Mathf.Deg2Rad / 2;
        if (Utility.AngleBetweenTwoVectors(player.head.forward, position - player.head.position) < angle && Utility.IsVisible(position + Vector3.up * 2, player.gameObject, float.MaxValue, player.Position, enemySpawnCheckMask)) 
            return false;

        return true;
    }

    public override bool SpawnPlayer()
    {
        return true;
    }

    public override void FailEnd()
    {
        gameOverText.text = PTS.ToString() + " PTS\n\n" + GetTimeString();
        gameOverMenu.SetActive(true);
        Menu.Instance.CanShow = false;
        Utility.EnableCursor();
    }

    public void ReloadScene()
    {
        SceneNameContainer.Instance.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
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

    public override void OnEnemyKilled(AICharacter enemy)
    {
        AddPTS(enemy.stats.PTSSurvival);
    }

    public override void OnPlayerDamaged()
    {
        PTSMultiplierMeter -= PTSMultiplierPerStage;
        if (PTSMultiplierMeter < 0)
            PTSMultiplierMeter = 0;
    }

    public override GameObject GetHUDElement()
    {
        return hudElement;
    }

    #region PTS
    private void AddPTS(int pts)
    {
        PTS += pts * PTSMultiplier;
        PTSMultiplierMeter += pts;
        if (PTSMultiplierMeter > (PTSMultiplierMaxStage + 0.9999f) * PTSMultiplierPerStage)
            PTSMultiplierMeter = (PTSMultiplierMaxStage + 0.9999f) * PTSMultiplierPerStage;

        PTSMultiplierDecayTimer = 0;
    }

    private void PTSMultiplierUpdate()
    {
        if (PTSMultiplierDecayTimer >= PTSMultiplierDecayDelay)
        {
            PTSMultiplierMeter -= Time.deltaTime * PTSMultiplierDecaySpeed;
            if (PTSMultiplierMeter < 0)
                PTSMultiplierMeter = 0;
        }
        else
        {
            PTSMultiplierDecayTimer += Time.deltaTime;
        }
    }

    public string GetTimeString()
    {
        string hours = ((int)(TimeCounter / 360)).ToString();
        string minutes = ((int)(TimeCounter / 60) % 60).ToString();
        string seconds = ((int)(TimeCounter % 60)).ToString();
        if (hours.Length == 1)
            hours = hours.Insert(0, "0");
        if (minutes.Length == 1)
            minutes = minutes.Insert(0, "0");
        if (seconds.Length == 1)
            seconds = seconds.Insert(0, "0");
        return hours + ":" + minutes + ":" + seconds;
    }

    #endregion
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

