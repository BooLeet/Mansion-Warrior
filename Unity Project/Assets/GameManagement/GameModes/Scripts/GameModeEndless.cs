using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeEndless : GameMode
{
    private List<PlayerSpawnerEndless> easySpawns = new List<PlayerSpawnerEndless>();
    private List<PlayerSpawnerEndless> mediumSpawns = new List<PlayerSpawnerEndless>();
    private List<PlayerSpawnerEndless> hardSpawns = new List<PlayerSpawnerEndless>();
    private PlayerSpawnerEndless startSpawn;

    public float enemySpawnDistance = 50;
    public LayerMask enemySpawnCheckMask;
    public int RoomCount { get; private set; }
    public uint consecutiveEasyRooms = 4;
    public uint consecutiveMediumRooms = 3;
    public uint consecutiveHardRooms = 2;
    private int GetCycleRoomCount { get { return (int)(consecutiveEasyRooms + consecutiveMediumRooms + consecutiveHardRooms); } }

    public Weapon testWeapon;

    private void Start()
    {
        RoomCount = 0;
        startSpawn.SpawnNewPlayer();
    }

    public void NextRoom()
    {
        GetNextPlayerSpawner().TeleportPlayer();
        GameManager.Instance.ActivateAllEnemySpawners();
        GameManager.Instance.ResetPickups();
    }

    private PlayerSpawnerEndless GetNextPlayerSpawner()
    {
        int cycleRoomCount = RoomCount % GetCycleRoomCount;
        if (cycleRoomCount < consecutiveEasyRooms)
        {
            return easySpawns[Random.Range(0, easySpawns.Count)];
        }
        else if (cycleRoomCount < consecutiveEasyRooms + consecutiveMediumRooms)
        {
            return mediumSpawns[Random.Range(0, mediumSpawns.Count)];
        }
        else
        {
            return hardSpawns[Random.Range(0, hardSpawns.Count)];
        }
    }

    public void RegisterPlayerSpawner(PlayerSpawnerEndless spawner)
    {
        switch(spawner.difficulty)
        {
            case PlayerSpawnerEndless.Difficulty.Easy: easySpawns.Add(spawner); break;
            case PlayerSpawnerEndless.Difficulty.Medium: mediumSpawns.Add(spawner); break;
            case PlayerSpawnerEndless.Difficulty.Hard: hardSpawns.Add(spawner); break;
            case PlayerSpawnerEndless.Difficulty.Start: startSpawn = spawner; break;
        }
    }

    public override void FailEnd()
    {
        SceneNameContainer.Instance.BeginLoading();
    }

    public override bool SpawnEnemy(Vector3 position)
    {
        if (PlayerCharacter.Instance == null)
            return false;

        if (Vector3.Distance(position, PlayerCharacter.Instance.Position) > enemySpawnDistance)
            return false;

        return Utility.IsVisible(position + Vector3.up, PlayerCharacter.Instance.gameObject, enemySpawnDistance, PlayerCharacter.Instance.verticalTargetingOffset, enemySpawnCheckMask);
    }

    public override bool SpawnPlayer()
    {
        return false;
    }

    public override void SuccessEnd()
    {
        
    }

    public override void OnEnemyDamaged()
    {
        
    }
}
