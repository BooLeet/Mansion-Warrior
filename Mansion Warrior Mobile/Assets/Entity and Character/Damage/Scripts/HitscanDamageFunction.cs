using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Damage/HitscanDamage")]
public class HitscanDamageFunction : DamageFunction
{
    public float damageRange = 200;
    public GameObject hitEffect;
    public uint hitscansPerCall = 1;
    public uint punchThroughtCount = 0;
    public LayerMask layerMask;

    public override void DoDamage(Character attacker, float damage)
    {
        bool sendDamageFeedback = false;
        Entity.DamageOutcome damageOutcome = Entity.DamageOutcome.Hit;
        for (uint i = 0; i < hitscansPerCall; ++i)
        {
            Vector3 attackDirection = attacker.GetAttackDirection();
            Vector3 attackSource = attacker.GetAttackSource();

            for (int j = 0; j < punchThroughtCount + 1; ++j)
            {
                Ray ray = new Ray(attackSource, attackDirection);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, damageRange, layerMask))
                {
                    attackSource = hitInfo.point;
                    Entity entity = hitInfo.collider.GetComponent<Entity>();
                    if (entity)
                    {
                        sendDamageFeedback = true;
                        Entity.DamageOutcome outcome = entity.TakeDamage(damage, attacker, hitInfo.point);
                        if (outcome > damageOutcome)
                            damageOutcome = outcome;

                    }
                    if (hitEffect)
                        Instantiate(hitEffect, hitInfo.point, Quaternion.identity).transform.LookAt(hitInfo.point + hitInfo.normal);
                }
            }
        }
        if(sendDamageFeedback)
            SendDamageFeedback(attacker, null, damageOutcome);
    }
}
