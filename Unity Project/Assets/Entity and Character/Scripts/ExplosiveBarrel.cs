using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : Entity
{
    public float maxHealth = 30;
    public float explosionRadius = 30;
    public float explosionDamage = 100;
    public GameObject explosionEffect;
    public AudioClip explosionSound;
    public LayerMask explosionLayerMask;
    public override float GetMaxHealth()
    {
        return maxHealth;
    }

    protected override void DeathEffect()
    {
        Instantiate(explosionEffect, transform.position + verticalTargetingOffset * transform.up, transform.rotation);
        Audio.PlaySFX(explosionSound, transform.position, null, 0.5f);
        CameraShaker.Shake(transform.position);
        Damage.ExplosiveDamage(Position, explosionDamage, explosionRadius, this, explosionLayerMask);
        Destroy(gameObject);
    }

    protected override float GetSelfDamageMultiplier()
    {
        return 0;
    }

    protected override void OnDamageTaken(float rawDamage, Entity damageGiver, Vector3 sourcePosition)
    {
        
    }
}
