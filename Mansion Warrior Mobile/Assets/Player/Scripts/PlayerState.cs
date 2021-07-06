using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState 
{
    public abstract void Init(PlayerCharacter player);

    public abstract void Action(PlayerCharacter player);

    public abstract PlayerState Transition(PlayerCharacter player);
}

namespace PlayerStates
{
    public class DefaultState : PlayerState
    {
        bool slide = false;
        Vector3 slideDirection;

        public override void Action(PlayerCharacter player)
        {
            player.Rotation(player.input.GetRotationInput());
            player.IsSprinting = player.input.GetSprint() && !player.IsFiring();
            Vector2 walkingInput = player.input.GetMovementInput();
            if (walkingInput.magnitude == 0)
                player.IsSprinting = false;

            if (player.IsSprinting)
                walkingInput.Normalize();

            //bool movingForward = walkingInput.magnitude > 0 && walkingInput.normalized.y > 0 && Mathf.Abs(walkingInput.normalized.x) < Mathf.PI / 4;
            if (player.input.GetSlide() && (walkingInput.magnitude > 0 || !player.IsNearGround()))//if (player.input.GetSlide() && (player.IsSprinting || !player.IsNearGround()) && movingForward)
            {
                slide = true;
                if (walkingInput.magnitude == 0)
                    slideDirection = player.transform.forward;
                else
                    slideDirection = player.transform.forward * walkingInput.y + player.transform.right * walkingInput.x;
            }

            player.ManualMovement(walkingInput, player.transform.forward, player.transform.right);
            if (player.input.GetJump())
                player.Jump();
                
            player.Gravity();
            player.ApplyMovement();

            if (player.input.GetInteract())
                player.Interact();

            if (player.input.GetSwapWeapon())
                player.SwapWeapons();

            if (player.input.GetReloadWeapon())
                player.ReloadWeaponAction();

            if (player.input.GetFireWeapon(player.PlayerInventory.CurrentWeapon.weapon))
                player.FireWeaponAction();

            if (player.input.GetInspectWeapon())
                player.InspectWeaponAction();

            if (player.input.GetPunch())
                player.MeleeAction();

            player.animator.ProceduralEffects(player.input.GetRotationInput(), player.input.GetMovementInput() * player.GetControllerVelocityCoefficient(player.GetMoveSpeed()));
        }

        public override void Init(PlayerCharacter player)
        {
            player.EnableGravity = true;
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (player.CanUseAbility() && player.input.GetAbility())
                return new SlamStartState();

            if (slide && player.IsNearGround())
                return new SlideState(slideDirection);

            return null;
        }
    }

    public class SlideState : PlayerState
    {
        float timeCounter = 0;
        Vector3 direction;
        bool cancelSlide = false;

        public SlideState(Vector3 slideDirection)
        {
            direction = slideDirection.normalized;
        }

        public override void Action(PlayerCharacter player)
        {
            // Movement
            float speed = Mathf.Lerp(0, player.stats.slideSpeed, timeCounter / player.stats.slideDuration);
            player.ManualMovementCustomSpeed(Vector2.up, direction, Vector3.zero, speed);
            if (player.input.GetJump())
            {
                player.Jump();
                cancelSlide = true;
            }
            player.Gravity();
            player.ApplyMovement();
            timeCounter -= Time.deltaTime;

            if (player.input.GetInteract())
                player.Interact();

            if (player.input.GetSwapWeapon())
                player.SwapWeapons();

            // Rotation
            player.Rotation(player.input.GetRotationInput());

            if (player.input.GetReloadWeapon())
                player.ReloadWeaponAction();

            // Firing
            if (player.input.GetFireWeapon(player.PlayerInventory.CurrentWeapon.weapon))
                player.FireWeaponAction();

            if (player.input.GetPunch())
                player.MeleeAction();

            player.animator.ProceduralEffects(player.input.GetRotationInput(), Vector2.zero);
        }

        public override void Init(PlayerCharacter player)
        {
            timeCounter = player.SlideDuration;
            player.ConsumeSlide();
            player.IsSprinting = false;
            player.input.ResetSprint();
            player.IsCrouching = true;
            player.animator.PlaySlideSound();
            CameraShaker.Shake(player.transform.position, 0.1f);
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (player.CanUseAbility() && player.input.GetAbility())
            {
                player.IsCrouching = false;
                return new SlamStartState();
            }

            if (timeCounter <= 0 || cancelSlide)
            {
                player.IsCrouching = false;
                return new DefaultState();
            }

            return null;
        }
    }

    public class SlamStartState : PlayerState
    {
        float timeCounter = 0;
        float ascendTime = 0.8f;
        float startVerticalAimAngle;

        public override void Action(PlayerCharacter player)
        {
            Vector2 movementInput = player.input.GetMovementInput() * player.stats.slamAscendMovementSpeed;
            Vector3 movement = player.transform.forward * movementInput.y + player.transform.right * movementInput.x + Vector3.up * (1 - timeCounter / ascendTime) * 30;

            player.JustMove(movement * Time.deltaTime);
            player.SetVerticalAimAngle(Mathf.Lerp(startVerticalAimAngle, player.maxVerticalRotationAngle, timeCounter / ascendTime));
            timeCounter += Time.deltaTime;
            player.animator.ProceduralEffects(Vector2.zero, Vector2.zero);
        }

        public override void Init(PlayerCharacter player)
        {
            player.IsInvincible = true;
            player.ResetInertia();
            player.animator.SlamStart();
            player.InteruptAction();
            player.CanAutoTarget = false;
            startVerticalAimAngle = player.VerticalAimAngle;
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (timeCounter > ascendTime)
                return new SlamEndState();
            return null;
        }
    }

    public class SlamEndState : PlayerState
    {
        public override void Action(PlayerCharacter player)
        {
            player.JustMove(100 * Vector3.down * Time.deltaTime);
        }

        public override void Init(PlayerCharacter player)
        {
            player.animator.SlamEnd();
        }

        public override PlayerState Transition(PlayerCharacter player)
        {
            if (player.IsNearGround())
            {
                player.SetVerticalAimAngle(0);
                player.IsInvincible = false;
                player.DoSlamDamage();
                player.animator.EquipWeapon(player.PlayerInventory.CurrentWeapon.weapon);
                player.animator.Idle();
                player.CanAutoTarget = true;
                return new DefaultState();
            }

            return null;
        }
    }

}

