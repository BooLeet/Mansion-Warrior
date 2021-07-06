using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : PlayerInput
{
    public float verticalRotationSpeedMultiplier = 0.7f;


    private int selectedWeaponSlot = 0;

    private float semiAutoPressTimeCounter = 0;
    public float semiAutoPressDuration = 0.2f;

    //private float punchPressTimeCounter = 0;
    //public float punchPressTimeDuration = 0.3f;

    private void Update()
    {
        WeaponSelectionUpdate();

        if (semiAutoPressTimeCounter > 0)
            semiAutoPressTimeCounter -= Time.deltaTime;
        if (Input.GetKeyDown(Settings.GetKeyCode("fire")))
            semiAutoPressTimeCounter = semiAutoPressDuration;


    }

    public override Vector2 GetRotationInput()
    {
        float cameraSensitivity = Settings.GetFloatValueSetting("settingMouseSensitivity").value;
        return new Vector2(Input.GetAxisRaw("Mouse X") * cameraSensitivity, Input.GetAxisRaw("Mouse Y") * cameraSensitivity * verticalRotationSpeedMultiplier);
    }

    public override Vector2 GetMovementInput()
    {
        Vector2 input = Vector2.zero; //new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetKey(Settings.GetKeyCode("forward")))
            input += Vector2.up;
        if (Input.GetKey(Settings.GetKeyCode("back")))
            input += Vector2.down;
        if (Input.GetKey(Settings.GetKeyCode("right")))
            input += Vector2.right;
        if (Input.GetKey(Settings.GetKeyCode("left")))
            input += Vector2.left;

        if (input.magnitude > 1)
            return input.normalized;

        return input;
    }

    public override bool GetJump()
    {
        return Input.GetKeyDown(Settings.GetKeyCode("jump"));
    }

    public override bool GetFireWeaponSemiAuto()
    {
        return semiAutoPressTimeCounter > 0;
    }

    public override bool GetFireWeaponFullAuto()
    {
        return Input.GetKey(Settings.GetKeyCode("fire"));
    }

    public override bool GetInteract()
    {
        return Input.GetKeyDown(Settings.GetKeyCode("interact"));
    }


    public override bool GetInspectWeapon()
    {
        return Input.GetKeyDown(Settings.GetKeyCode("inspect"));
    }

    public override bool GetSlide()
    {
        return Input.GetKeyDown(Settings.GetKeyCode("slide"));
    }

    private void WeaponSelectionUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            selectedWeaponSlot = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            selectedWeaponSlot = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            selectedWeaponSlot = 2;
    }

    public override bool GetPunch()
    {
        return Input.GetKeyDown(Settings.GetKeyCode("melee"));
    }

    public override bool GetPause()
    {
        return Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape);
    }

    public override string GetInteractionKey()
    {
        return Localizer.LocalizeKeyCode(Settings.GetKeyCode("interact"));
    }

    public override bool AutoTargetEnabled()
    {
        return false;
    }

    public override bool GetReloadWeapon()
    {
        return Input.GetKeyDown(Settings.GetKeyCode("reload"));
    }

    protected override bool GetSprintPress()
    {
        return Input.GetKeyDown(Settings.GetKeyCode("sprint"));
    }

    protected override bool GetSprintHold()
    {
        return Input.GetKey(Settings.GetKeyCode("sprint"));
    }

    public override bool GetSwapWeapon()
    {
        return Input.GetKey(Settings.GetKeyCode("swapWeapon"));
    }

    public override bool GetAbility()
    {
        return Input.GetKey(Settings.GetKeyCode("ability"));
    }
}
