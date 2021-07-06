using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Damage
{
    public static void ExplosiveDamage(Vector3 origin, float damage, float range, Entity damagingEntity, int layerMask = int.MaxValue, bool checkVisibility = true)//float explosionForce
    {
        var closestVisibleEntities = from entity in EntityRegistry.GetInstance().GetClosestEntities(origin, range, damagingEntity)
                                     where !checkVisibility || Utility.IsVisible(origin, entity.gameObject, range, entity.Position, layerMask)
                                     select entity;

        foreach (Entity e in closestVisibleEntities)
        {
            float distance = Vector3.Distance(e.Position, origin);
            float distanceMultiplier = (distance / range) > 0.5f ? (distance / range) : 1;

            e.TakeDamage(damage * distanceMultiplier, damagingEntity, origin);
            //SendDamageFeedback(damagingEntity, e, e.TakeDamage(damage * distanceMultiplier, damagingEntity, origin));
        }
    }
}
