using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInputController : PlayerInput
{
    private float cameraSensitivityMultiplier = 100;
    public float moveInputMultiplier = 1;
    //public Canvas canvas;
    //[Space]
    public MobileInputJoy moveJoystick;
    public MobileInputJoy primaryJoystick;
    public MobileInputJoy secondaryJoystick;
    public MobileInputButton cycleWeaponButton;
    public MobileInputButton dashButton;
    public MobileInputButton jumpButton;
    public MobileInputButton interactButton;
    public MobileInputButton pauseButton;

    public MobileInputCamera cameraInput;

    private List<MobileInputElement> elements;
    private bool pauseWasPressed = false;

    private void Start()
    {
        elements = new List<MobileInputElement>();
        elements.Add(moveJoystick);
        elements.Add(primaryJoystick);
        elements.Add(secondaryJoystick);
        elements.Add(dashButton);
        elements.Add(interactButton);
        elements.Add(jumpButton);
        elements.Add(cycleWeaponButton);
        elements.Add(pauseButton);

        elements.Add(cameraInput);
    }

    void Update()
    {
        pauseWasPressed = pauseButton.KeyDown;

        foreach (Touch touch in Input.touches)
        {
            foreach (MobileInputElement element in elements)
            {
                if (element.HandleTouch(touch))
                    break;
            }
        }

        //if (cycleWeaponButton.KeyDown)
        //{
        //    int weaponCount = player.PlayerInventory.GetWeaponSlotCount();
        //    int nextWeaponSlot = selectedWeaponSlot;

        //    for (int i = 0; i < weaponCount; ++i)
        //    {
        //        nextWeaponSlot++;
        //        nextWeaponSlot %= weaponCount;

        //        if (player.PlayerInventory.GetWeapon(nextWeaponSlot) != null)
        //        {
        //            selectedWeaponSlot = nextWeaponSlot;
        //            break;
        //        }
        //    }
        //}
    }

    public void ShowHide(bool val)
    {
        gameObject.SetActive(val);
    }

    public override bool GetSlide()
    {
        return dashButton.KeyDown;
    }

    public override bool GetFireWeaponFullAuto()
    {
        return primaryJoystick.IsPressed;
    }

    public override bool GetFireWeaponSemiAuto()
    {
        return primaryJoystick.IsPressed;
    }


    public override bool GetInspectWeapon()
    {
        return false;
    }

    public override bool GetInteract()
    {
        return interactButton.KeyDown;
    }

    public override bool GetJump()
    {
        return jumpButton.KeyDown;
    }

    public override Vector2 GetMovementInput()
    {
        Vector3 input = moveJoystick.Input * moveInputMultiplier;
        if (input.magnitude > 1)
            input.Normalize();
        return input.normalized;// * Mathf.Pow(input.magnitude, 1 / 4f);
    }

    public override bool GetPunch()
    {
        return secondaryJoystick.IsPressed;
    }

    public override Vector2 GetRotationInput()
    {
        float cameraSensitivity = Settings.GetFloatValueSetting("settingMouseSensitivity").value;
        return cameraSensitivityMultiplier * cameraSensitivity * (cameraInput.DeltaInput + primaryJoystick.DeltaInput + secondaryJoystick.DeltaInput) / Screen.height;
    }

    public override bool GetPause()
    {
        if(pauseButton.KeyDown)
        {
            pauseButton.ClearPresses();
            return true;
        }
        return false;
    }

    public override string GetInteractionKey()
    {
        return "";
    }

    public override bool AutoTargetEnabled()
    {
        return true;
    }

    public override bool GetReloadWeapon()
    {
        return false;
    }

    protected override bool GetSprintHold()
    {
        throw new System.NotImplementedException();
    }

    protected override bool GetSprintPress()
    {
        throw new System.NotImplementedException();
    }

    protected override bool HoldToSprint()
    {
        return false;
    }
    public override bool GetSwapWeapon()
    {
        throw new System.NotImplementedException();
    }

    public override bool GetAbility()
    {
        throw new System.NotImplementedException();
    }
}
