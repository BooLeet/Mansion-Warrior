using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Character/Stats/AI")]
public class AIStats : CharacterStats
{
    [Header("Attack")]
    public DamageFunction damageFunction;
    public AIDirector.TokenType attackTokenType;
    public float attackDamage = 10;

    [Range(0, 100)]
    public float attackDistance = 5f;
    [Space]
    [Range(0, 10)]
    public float attackTokenCooldownTime = 1;
    [Range(0, 20)]
    public uint attacksPerToken = 1;

    [Header("Detection system")]
    public bool alwaysAlert = false;
    public LayerMask visibleLayers;
    public float alarmDelay = 1f;
    [Range(0, 100)]
    public float visibilityDistance = 25f;
    [Range(0, 180)]
    public float visibilityAngle = 90;
    public float cantFindTimeThreshold = 60;

    [Header("Loot")]
    public GameObject[] loot;
    [Range(0,1)]
    public float lootDropChance = 0.5f;

    public GameObject GetRandomLoot()
    {
        return loot[Random.Range(0, loot.Length)];
    }
}
