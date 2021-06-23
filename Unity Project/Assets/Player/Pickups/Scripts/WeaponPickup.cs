using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : ResourcePickup
{
    public Weapon weapon;
    public uint loadedAmmo;
    public uint ammoCount;
    [Space]
    public Transform weaponTransform;
    public float rotationSpeed;
    public float wobbleSpeed = 1;
    public float maxWobbleHeight = 0.3f;
    private float wobbleParameter;
    private float startingHeight;

    private void Start()
    {
        startingHeight = weaponTransform.localPosition.y;
    }

    protected override void UpdateFunction()
    {
        weaponTransform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
        weaponTransform.transform.localPosition = new Vector3(0, startingHeight + maxWobbleHeight * Mathf.Sin(wobbleParameter));
        wobbleParameter += Time.deltaTime;
    }

    protected override void AddResource(PlayerCharacter player)
    {
        if (!player.PlayerInventory.HasWeapon(weapon)) 
            player.EquipWeapon(weapon, loadedAmmo);
        player.AddAmmo(weapon.ammoNameKey, ammoCount);
    }

    protected override bool CanAddResource(PlayerCharacter player)
    {
        return player.CanAddAmmo(weapon.ammoNameKey) || !player.HasWeapon(weapon);
    }
}
