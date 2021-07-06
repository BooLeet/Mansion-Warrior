using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnerEndless : MonoBehaviour
{
    public enum Difficulty { Easy,Medium,Hard, Start}
    public Difficulty difficulty;

    public GameObject playerPrefab;
    private GameModeEndless gameMode;

    private void Start()
    {
        gameMode = GameMode.Instance as GameModeEndless;
        if(gameMode)
            gameMode.RegisterPlayerSpawner(this);
    }

    public void SpawnNewPlayer()
    {
        Instantiate(playerPrefab, transform.position, transform.rotation);
    }

    public void TeleportPlayer()
    {
        PlayerCharacter.Instance.Warp(transform.position);
        PlayerCharacter.Instance.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
}
