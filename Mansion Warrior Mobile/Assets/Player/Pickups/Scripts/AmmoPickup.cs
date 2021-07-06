using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : ResourcePickup
{
    [System.Serializable]
    public class AmmoInfo
    {
        public string nameKey;
        public uint count;
    }

    public AmmoInfo[] ammoInfo;

    protected override void AddResource(PlayerCharacter player)
    {
        foreach (var ammo in ammoInfo) 
            player.AddAmmo(ammo.nameKey, ammo.count);
    }

    protected override bool CanAddResource(PlayerCharacter player)
    {
        bool canAdd = false;
        for (int i = 0; i < ammoInfo.Length && !canAdd; ++i)
            canAdd = player.CanAddAmmo(ammoInfo[i].nameKey);

        return canAdd;
    }
}
