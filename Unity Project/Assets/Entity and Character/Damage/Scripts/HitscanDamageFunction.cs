using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Damage/HitscanDamage")]
public class HitscanDamageFunction : DamageFunction
{
    public float damageRange = 200;
    public GameObject hitEffect;
    public uint hitscansPerCall = 1;
    public LayerMask layerMask;

    public override void DoDamage(Character attacker, float damage)
    {
        for (uint i = 0; i < hitscansPerCall; ++i)
        {
            Ray ray = new Ray(attacker.GetAttackSource(), attacker.GetAttackDirection());
            if (Physics.Raycast(ray, out RaycastHit hitInfo, damageRange, layerMask))
            {
                Entity entity = hitInfo.collider.GetComponent<Entity>();
                if (entity)
                    SendDamageFeedback(attacker, entity, entity.TakeDamage(damage, attacker, hitInfo.point));
                if (hitEffect)
                    Instantiate(hitEffect, hitInfo.point, Quaternion.identity).transform.LookAt(hitInfo.point + hitInfo.normal);
            }
        }
    }
}
