using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Player/Stats", fileName = "PlayerStats")]
public class PlayerStats : CharacterStats
{
    public float jumpVelocityRecoveryTime = 1;
    [Space]
    public float sprintSpeedMultiplier = 1.75f;
    public float inAirSpeed = 2f;
    public float inertiaResistanceStrength = 1f;

    [Header("Slide")]
    public float slideSpeed = 15f;
    public float slideDuration = 1f;
    public float slideDurationRecoveryTime = 1.5f;

    [Header("Melee")]
    public DamageFunction meleeFunction;
    public float meleeDamage = 5;

    [Header("Recoil")]
    public float maxRecoilParameter = 5;
    public float recoilRecoveryParameter = 10;

    [Header("Weapons")]
    public Weapon primaryWeapon;
    public Weapon secondaryWeapon;
    public List<PlayerInventory.AmmoContainer> ammoCaps;
    public List<PlayerInventory.AmmoContainer> startingAmmo;

    [Header("Interaction")]
    public float interactionDistance = 3;
    public float interactionAngle = 90;
    public float pickupDistance = 1.5f;

    [Header("Auto Targetting")]
    public float autoTargetRange = 50;
    [Range(0,180)]
    public float autoTargetAngle = 90;
    public LayerMask autoTargetLayerMask;

    [Header("Ability")]
    public float abilityMaxCharge = 3;
    public float abilityChargeFillSpeed = 0.1f;
    [Space]
    public LayerMask slamLayerMask;
    public float slamDamage = 100;
    public float slamRange = 30;
    public float slamAscendMovementSpeed = 3;
    public GameObject slamExplosionEffect;

    public uint GetAmmoCap(string nameKey)
    {
        var found = from ammoContainer in ammoCaps
                    where ammoContainer.nameKey == nameKey
                    select ammoContainer.count;

        if (found.Count() == 0)
            return 0;
        return found.First();
    }
}
