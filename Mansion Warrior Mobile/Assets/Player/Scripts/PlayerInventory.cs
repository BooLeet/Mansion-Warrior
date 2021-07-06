using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInventory
{
    [System.Serializable]
    public class AmmoContainer
    {
        public string nameKey;
        public uint count;

        public AmmoContainer(string nameKey, uint count)
        {
            this.nameKey = nameKey;
            this.count = count;
        }
    }

    private PlayerCharacter player;
    private List<AmmoContainer> currentAmmo;
    public class EquippedWeapon
    {
        public Weapon weapon;
        public uint loadedAmmo { get; private set; }

        public EquippedWeapon(Weapon weapon, uint loadedAmmo)
        {
            this.weapon = weapon;
            this.loadedAmmo = loadedAmmo;
        }

        public bool Equals(EquippedWeapon other)
        {
            return other != null && other.weapon == weapon && other.loadedAmmo == loadedAmmo;
        }

        public bool IsFullyLoaded()
        {
            return loadedAmmo == weapon.magazineSize;
        }

        public bool IsEmpty()
        {
            return loadedAmmo == 0;
        }

        public bool ReloadNeeded()
        {
            return loadedAmmo >= weapon.ammoPerShot;
        }

        public void Shoot(PlayerCharacter player)
        {
            loadedAmmo -= weapon.ammoPerShot;
            weapon.damageFunction.DoDamage(player, weapon.damage);
        }

        public void Reload(PlayerInventory playerInventory)
        {
            uint availableAmmo = playerInventory.GetAmmoCount(weapon.ammoNameKey);
            uint ammoToLoad = (uint)Mathf.Min(availableAmmo, weapon.bulletsPerReload);
            if (ammoToLoad + loadedAmmo > weapon.magazineSize)
                ammoToLoad  = weapon.magazineSize - loadedAmmo;

            loadedAmmo += ammoToLoad;
            playerInventory.SpendAmmo(weapon.ammoNameKey, ammoToLoad);
        }
    }

    public EquippedWeapon CurrentWeapon { get; private set; }
    public EquippedWeapon HolsteredWeapon { get; private set; }

    public PlayerInventory(PlayerCharacter player)
    {
        this.player = player;
        currentAmmo = new List<AmmoContainer>();
    }


    #region Ammo
    public void AddAmmo(List<AmmoContainer> ammo)
    {
        foreach (AmmoContainer container in ammo)
            AddAmmo(container.nameKey, container.count);
    }

    public uint GetAmmoCount(string nameKey)
    {
        var found = from ammoContainer in currentAmmo
                    where ammoContainer.nameKey == nameKey
                    select ammoContainer.count;

        if (found.Count() == 0)
            return 0;
        return found.First();
    }

    public bool CanAddAmmo(string nameKey)
    {
        return GetAmmoCount(nameKey) < player.stats.GetAmmoCap(nameKey);
    }

    public void AddAmmo(string nameKey, uint count)
    {
        var found = from ammoContainer in currentAmmo
                    where ammoContainer.nameKey == nameKey
                    select ammoContainer;

        uint ammoCap = player.stats.GetAmmoCap(nameKey);
        if (found.Count() == 0)
            currentAmmo.Add(new AmmoContainer(nameKey, (uint)Mathf.Min(count, ammoCap) ));
        else
            found.First().count = (uint)Mathf.Min(found.First().count + count, ammoCap);
    }

    public void SpendAmmo(string nameKey, uint count)
    {
        var found = from ammoContainer in currentAmmo
                    where ammoContainer.nameKey == nameKey
                    select ammoContainer;

        if (found.Count() == 0)
            return;

        int newCount = (int)found.First().count - (int)count;
        found.First().count = (uint)Mathf.Max(newCount, 0);
    }

    #endregion

    #region Weapons

    public void EquipWeapon(EquippedWeapon newWeapon)
    {
        EquipWeapon(newWeapon.weapon, newWeapon.loadedAmmo);
    }

    public void EquipWeapon(Weapon weapon, uint loadedAmmo)
    {
        if (weapon == null)
            return;

        EquippedWeapon newWeapon = new EquippedWeapon(weapon, loadedAmmo);

        if (newWeapon.Equals(HolsteredWeapon))
        {
            HolsteredWeapon = CurrentWeapon;
            CurrentWeapon = newWeapon;
            return;
        }

        if (CurrentWeapon == null)
            CurrentWeapon = newWeapon;
        else if (HolsteredWeapon == null)
        {
            HolsteredWeapon = CurrentWeapon;
            CurrentWeapon = newWeapon;
        }
        else
            CurrentWeapon = newWeapon;
    }

    public void SwapWeapons()
    {
        if (HolsteredWeapon == null)
            return;
        EquippedWeapon temp = CurrentWeapon;
        CurrentWeapon = HolsteredWeapon;
        HolsteredWeapon = temp;
    }

    public bool HasWeapon(Weapon weapon)
    {
        return (CurrentWeapon != null && weapon == CurrentWeapon.weapon) || (HolsteredWeapon != null && weapon == HolsteredWeapon.weapon);
    }

    #endregion
}
