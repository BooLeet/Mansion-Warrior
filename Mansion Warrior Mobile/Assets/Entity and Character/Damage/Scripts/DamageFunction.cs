using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageFunction : ScriptableObject
{
    public abstract void DoDamage(Character attacker, float damage);

    public static void SendDamageFeedback(Character reciever, Entity damagedEntity, Entity.DamageOutcome damageOutcome)
    {
        if (reciever)
            reciever.DamageFeedback(damagedEntity, damageOutcome);
    }
}
