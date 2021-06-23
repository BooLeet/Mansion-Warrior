using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Weapon")]
public class Weapon : ScriptableObject
{
    [Header("Damage")]
    public DamageFunction damageFunction;
    public float damage;
    public bool fullAuto;
    public enum WeaponType { Pistol, MachineGun, Shotgun}
    public WeaponType type;
    public GameObject rightPrefab;
    public GameObject leftPrefab;

    [Header("Recoil")]
    public float recoilDuration = 0.1f;
    public float recoilStrength = 5;
    public float recoilAngle = 0.5f;
    [Header("Spread")]
    public float spreadAngle = 5;
    public float spreadRecoilMultiplier = 1f;
    [Space]
    public string ammoNameKey;
    public uint ammoPerShot = 1;
    public uint magazineSize = 30;
    public uint bulletsPerReload = 30;
}
