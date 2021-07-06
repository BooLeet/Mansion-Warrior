using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : ResourcePickup
{
    public float healAmount = 5;

    protected override void AddResource(PlayerCharacter player)
    {
        player.GiveHealth(healAmount);
    }

    protected override bool CanAddResource(PlayerCharacter player)
    {
        return player.CurrentHealth < player.GetMaxHealth();
    }
}
