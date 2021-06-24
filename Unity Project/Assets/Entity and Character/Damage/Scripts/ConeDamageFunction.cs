using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Damage/ConeDamage")]
public class ConeDamageFunction : DamageFunction
{
    public float coneOffset = 2;
    [Range(0,100)]
    public float range = 3;
    [Range(0, 180)]
    public float coneAngle = 90;
    public uint maxHitCount = 10;
    public LayerMask layerMask;

    public override void DoDamage(Character attacker, float damage)
    {
        Vector3 direction = attacker.GetAttackDirection();
        Vector3 origin = attacker.GetAttackSource();
        int enemiesToHit = (int)maxHitCount;

        Vector3 offsetOrigin = origin - direction.normalized * coneOffset;
        var closestEntitiesWithinAngle = from entity in EntityRegistry.GetInstance().GetClosestEntities(offsetOrigin, range + coneOffset, attacker)
                                         where Utility.WithinAngle(offsetOrigin, direction, entity.Position, coneAngle) && Vector3.Distance(entity.Position, offsetOrigin) >= coneOffset &&
                                         Utility.IsVisible(origin, entity.gameObject, range, entity.Position, layerMask)
                                         select new { entity, distance = Vector3.Distance(entity.Position, origin) };

        IEnumerable<Entity> entitiesToDamage = from entityInfo in closestEntitiesWithinAngle
                                               orderby entityInfo.distance
                                               select entityInfo.entity;


        // if entity's Position didn't get into the cone
        Ray forwardRay = new Ray(origin, direction);
        RaycastHit rayHit;
        if (Physics.Raycast(forwardRay, out rayHit, range, layerMask)) 
        {
            Entity hitEntity = rayHit.collider.GetComponent<Entity>();
            if (hitEntity && !entitiesToDamage.Contains(hitEntity))
            {
                SendDamageFeedback(attacker, hitEntity, hitEntity.TakeDamage(damage, attacker, origin));
                --enemiesToHit;
            }
        }


        for (int i = 0; i < Mathf.Min(enemiesToHit, entitiesToDamage.Count()); ++i)
        {
            Entity entity = entitiesToDamage.ElementAt(i);
            SendDamageFeedback(attacker, entity, entity.TakeDamage(damage, attacker, origin));
        }
    }
}
