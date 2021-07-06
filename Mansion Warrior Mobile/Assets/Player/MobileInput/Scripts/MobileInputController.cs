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
    public MobileInputJoy fireJoystick;
    public MobileInputButton reloadButton;
    public MobileInputButton abilityButton;
    public MobileInputButton slideButton;
    public MobileInputButton interactButton;
    public MobileInputButton pauseButton;

    public MobileInputCamera cameraInput;

    private List<MobileInputElement> elements;

    private void Start()
    {
        elements = new List<MobileInputElement>();
        elements.Add(moveJoystick);
        //elements.Add(primaryJoystick);
        elements.Add(fireJoystick);
        //elements.Add(reloadButton);
        elements.Add(abilityButton);
        elements.Add(interactButton);
        elements.Add(slideButton);
        elements.Add(reloadButton);
        elements.Add(pauseButton);

        elements.Add(cameraInput);
    }

    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            foreach (MobileInputElement element in elements)
            {
                if (element.HandleTouch(touch))
                    break;
            }
        }
    }

    public void ResetInputs()
    {
        foreach (MobileInputElement element in elements)
            element.ResetInput();
    }

    public void ShowHide(bool val)
    {
        gameObject.SetActive(val);
    }

    public override bool GetSlide()
    {
        return slideButton.KeyDown;
    }

    public override bool GetFireWeaponFullAuto()
    {
        //return player.AutoTargetedEntity != null;
        return fireJoystick.IsPressed;
        //return primaryJoystick.IsPressed;
    }

    public override bool GetFireWeaponSemiAuto()
    {
        //return player.AutoTargetedEntity != null;
        return fireJoystick.IsPressed;
        //return primaryJoystick.IsPressed;
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
        return false;
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
        //return secondaryJoystick.IsPressed;
        return false;
    }

    public override Vector2 GetRotationInput()
    {
        float cameraSensitivity = Settings.GetFloatValueSetting("settingMouseSensitivity").value;
        return cameraSensitivityMultiplier * cameraSensitivity * (cameraInput.DeltaInput + fireJoystick.DeltaInput) / Screen.height;
        //return cameraSensitivityMultiplier * cameraSensitivity * (cameraInput.DeltaInput + primaryJoystick.DeltaInput + secondaryJoystick.DeltaInput) / Screen.height;
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
        return reloadButton.KeyDown;
    }

    protected override bool GetSprintHold()
    {
        return true;
    }

    protected override bool GetSprintPress()
    {
        return false;
    }

    public override bool GetSwapWeapon()
    {
        return false;
    }

    public override bool GetAbility()
    {
        return abilityButton.KeyDown;
    }
}
