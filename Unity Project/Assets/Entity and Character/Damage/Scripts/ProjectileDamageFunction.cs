using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Damage/ProjectileDamage")]
public class ProjectileDamageFunction : DamageFunction
{
    public float speed = 30f;
    public float radius = 0.5f;
    public GameObject graphic;
    public GameObject hitEffect;
    public int projectileLayer = 17;

    public override void DoDamage(Character attacker, float damage)
    {
        Projectile.StartProjectile(attacker, attacker.GetAttackSource(), attacker.GetAttackDirection(), damage, speed, radius, graphic, hitEffect, projectileLayer);
    }
}
