using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeBase : GameMode
{
    public HUD_GameFinish gameFinish;

    public override bool SpawnEnemy(Vector3 position)
    {
        return true;
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
        PlayerCharacter.Instance.FinishGame();
        gameFinish.StartEffect();
    }

    public override void OnEnemyDamaged()
    {
        
    }

    public override GameObject GetHUDElement()
    {
        return null;
    }

    public override void OnEnemyKilled(AICharacter enemy)
    {
        
    }

    public override void OnPlayerDamaged()
    {
        
    }
}
