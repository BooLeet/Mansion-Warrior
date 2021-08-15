using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInput : MonoBehaviour
{
    public PlayerCharacter player;
    private bool isSprinting = false;

    public abstract Vector2 GetRotationInput();

    public abstract Vector2 GetMovementInput();

    public abstract bool GetJump();

    public bool GetFireWeapon(Weapon weapon)
    {
        if (weapon == null)
            return GetFireWeaponFullAuto();

        if (weapon.fullAuto)
            return GetFireWeaponFullAuto();
        else
            return GetFireWeaponSemiAuto();
    }

    public abstract bool GetFireWeaponSemiAuto();

    public abstract bool GetFireWeaponFullAuto();

    public abstract bool GetInteract();

    public abstract bool GetSwapWeapon();

    public abstract bool GetReloadWeapon();

    public abstract bool GetInspectWeapon();

    public abstract bool GetSlide();

    public abstract bool GetPause();

    public abstract string GetInteractionKey();

    public abstract bool GetPunch();

    public abstract bool AutoTargetEnabled();

    protected abstract bool GetSprintPress();

    protected abstract bool GetSprintHold();

    protected abstract bool HoldToSprint();

    private bool AlwaysSprint()
    {
        Settings.SettingStringValue setting = Settings.GetStringValueSetting("settingAlwaysSprint");
        return setting != null && setting.value == "on";
    }

    public bool GetSprint()
    {
        //Vector2 moveInput = GetMovementInput();
        //bool movingForward = moveInput.magnitude > 0 && moveInput.normalized.y > 0 && Mathf.Abs(moveInput.normalized.x) < Mathf.PI / 4;

        if (AlwaysSprint())
            return true;

        if (HoldToSprint())// && movingForward)
            isSprinting = GetSprintHold();
        else
        {
            if (GetMovementInput().magnitude == 0 || GetFireWeaponSemiAuto() || GetFireWeaponFullAuto())// || !movingForward)
                isSprinting = false;
            else if (GetSprintPress())
                isSprinting = !isSprinting;
        }


        return isSprinting;
    }

    public void ResetSprint()
    {
        isSprinting = false;
    }

    public abstract bool GetAbility();
}
