using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    public Transform attackDirector;
    public PlayerCharacter player;

    [System.Serializable]
    public struct WeaponSet
    {
        public string idle;
        public string inspect;
        public string fire;
        public string reload;
        [Space]
        public string reloadInit;
        public string reloadEnd;
        [Space]
        public string equip;
        public string holster;
        [Header("Durations")]
        public float fireDuration;
        public float reloadInitDuration;
        public float reloadDuration;
        public float reloadEndDuration;
        public float equipDuration;
        public float holsterDuration;

        [Header("Audio")]
        public AudioClip fireSound;
        public AudioClip[] miscSounds;
    }

    [System.Serializable]
    public struct Action
    {
        public string trigger;
        public float duration;
    }

    [Header("Weapons")]
    public WeaponSet pistolSet;
    public WeaponSet machineGunSet;
    public WeaponSet shotgunSet;

    private WeaponSet currentSet;
    public Transform rightSlot, leftSlot;
    private Animator rightWeaponAnim, leftWeaponAnim;

    [Header("Rotation Effect")]
    public Transform armsHolder;
    private Vector3 armsHolderPivot;
    public float horizontalSpeed = 1;
    public float verticalalSpeed = 1;
    public float maxHorizontalOffset = 0.2f;
    public float maxVerticalOffset = 0.2f;

    [Header("Walking Effect")]
    public Transform cameraHolder;
    public float walkingSpeed = 10;
    public float sprintSpeed = 12;
    public float walkingEffectMagnitude = 5;
    public float walkingArmOffsetMagnitude = 0.02f;
    public float walkingHeadBobMagnitude = 0.2f;
    public float walkingLerpParameter = 5;
    private float walkingParameter;
    public AudioClip footstepSound;
    public AudioClip slideSound;

    [Header("Jumping and Landing")]
    public float inAirArmsAngle = -1;
    public float landingArmsAngle = 1;
    private float currentLandingArmsAngle = 0;

    [Header("Recoil")]
    public float maxRecoilOffset = 0.02f;
    public float weaponRecoilEffectMagnitude = 20;

    [Header("Aiming")]
    public Vector3 aimingArmOffset;

    private bool wasGrounded = true;

    [Header("Punch")]
    public string[] punchTriggers;
    private int punchTriggerIndex;
    public float punchDuration;
    public AudioClip punchSound;
    public GameObject punchHitEffect;

    [Header("Death")]
    public string deathTrigger = "Death";

    [Header("Ability")]
    public string slamStartTrigger = "SlamStart";
    public string slamEndTrigger = "SlamEnd";
    public AudioClip slamStartSound;
    public AudioClip slamEndSound;

    [Space]
    public AudioClip playerDamageSound;
    public GameObject dashEffect;

    private void Start()
    {
        armsHolderPivot = armsHolder.localPosition;

        //currentSet = shotgunSet;
        Idle();
    }

    #region Weapon Animation Calls

    public void Idle()
    {
        SetTrigger(currentSet.idle);
        SetWeaponAnimationTriggers(rightWeaponAnim, "Idle");
        SetWeaponAnimationTriggers(leftWeaponAnim, "Idle");
    }

    public void FireWeapon()
    {
        SetTrigger(currentSet.fire);
        SetWeaponAnimationTriggers(rightWeaponAnim, "Fire");
        SetWeaponAnimationTriggers(leftWeaponAnim, "Fire");
    }

    public void InspectWeapon()
    {
        SetTrigger(currentSet.inspect);
    }

    public void ReloadInit()
    {
        SetTrigger(currentSet.reloadInit);
    }

    public void ReloadWeapon()
    {
        SetTrigger(currentSet.reload);
        SetWeaponAnimationTriggers(rightWeaponAnim, "Reload");
        SetWeaponAnimationTriggers(leftWeaponAnim, "Reload");
    }


    public void ReloadEnd()
    {
        SetTrigger(currentSet.reloadEnd);
    }

    public void Equip()
    {
        SetTrigger(currentSet.equip);
    }

    public void Holster()
    {
        SetTrigger(currentSet.holster);
    }

    private void SetWeaponAnimationTriggers(Animator animator, string trigger)
    {
        if (animator == null)
            return;
        animator.SetTrigger(trigger);
    }
    #endregion

    public WeaponSet GetWeaponSet(Weapon weapon)
    {
        switch (weapon.type)
        {
            case Weapon.WeaponType.Pistol: return pistolSet;
            case Weapon.WeaponType.MachineGun: return machineGunSet;
            case Weapon.WeaponType.Shotgun: return shotgunSet;
            default: return pistolSet;
        }
    }

    public void EquipWeapon(Weapon weapon)
    {
        string rightObjectName = "";
        string leftObjectName = "";
        switch (weapon.type)
        {
            case Weapon.WeaponType.Pistol: currentSet = pistolSet;rightObjectName = "Pistol" ;break;
            case Weapon.WeaponType.MachineGun: currentSet = machineGunSet; rightObjectName = "MachineGun"; break;
            case Weapon.WeaponType.Shotgun: currentSet = shotgunSet;rightObjectName = "Shotgun";leftObjectName = "ShotgunShell"; break;
        }

        Utility.RemoveChildren(rightSlot);
        Utility.RemoveChildren(leftSlot);

        if (weapon.rightPrefab)
        {
            GameObject obj = Instantiate(weapon.rightPrefab, rightSlot);
            obj.name = rightObjectName;
            rightWeaponAnim = obj.GetComponent<Animator>();
        }
        if (weapon.leftPrefab)
        {
            GameObject obj = Instantiate(weapon.leftPrefab, leftSlot);
            obj.name = leftObjectName;
            leftWeaponAnim = obj.GetComponent<Animator>();
        }
    }

    public void UnequipWeapons()
    {
        Utility.RemoveChildren(rightSlot);
        Utility.RemoveChildren(leftSlot);
    }

    private void SetTrigger(string trigger)
    {
        if (trigger == "" || trigger == null)
            return;
        animator.SetTrigger(trigger);
    }

    public WeaponSet GetCurrentSet()
    {
        return currentSet;
    }

    #region Reloading

    public void ReloadWeaponCall()
    {
        player.ReloadCurrentWeapon();
    }


    #endregion

    #region Punch
    public void Punch()
    {
        animator.SetTrigger(punchTriggers[punchTriggerIndex++]);
        punchTriggerIndex %= punchTriggers.Length;
    }

    public void PlayPunchHitEffect()
    {
        if (punchHitEffect)
            Instantiate(punchHitEffect, transform.position + transform.forward, transform.rotation);
    }

    public void PunchDamageCall()
    {
        player.MeleeDamageCall();
    }
    #endregion

    #region Procedural Animations

    public void ProceduralEffects(Vector2 rotationAngles, Vector2 walkingInput)
    {
        float walkingMagnitude = walkingInput.magnitude;
        float previousWalkingParameter = walkingParameter;

        float parameterSpeedMultiplier = IsSprinting() ? sprintSpeed : walkingSpeed;

        if (player.IsNearGround())
            walkingParameter += Time.deltaTime * parameterSpeedMultiplier * walkingMagnitude;
        walkingParameter %= 2 * Mathf.PI;

        bool footstep = false;
        if (walkingParameter < previousWalkingParameter)
            footstep = true;
        else if (previousWalkingParameter <= Mathf.PI && walkingParameter > Mathf.PI)
            footstep = true;

        if (footstep)
            PlayFootstepSound();

        float sprintAngleMultiplier = IsSprinting() ? 2 : 1;
        float sprintCameraMultiplier = IsSprinting() ? 2 : 1;
        Vector3 sprintAngleOffset = (!IsReloading() && IsSprinting()) ? new Vector3(2, -5, 0) : Vector3.zero;

        Vector3 armsAngle = new Vector3(Mathf.Abs(Mathf.Cos(walkingParameter) * sprintAngleMultiplier), Mathf.Sin(walkingParameter) * sprintAngleMultiplier, walkingMagnitude);

        Vector2 recoilAngles = weaponRecoilEffectMagnitude * player.GetRecoilAngles();
        Quaternion targetArmRotation = Quaternion.Euler(-recoilAngles.y, recoilAngles.x, 0);
        if (!IsFiring())
            targetArmRotation = Quaternion.Euler(walkingEffectMagnitude * walkingMagnitude * (armsAngle + sprintAngleOffset));

        // Walking offset and weapon sway
        rotationAngles.x *= horizontalSpeed;
        rotationAngles.y *= verticalalSpeed;

        rotationAngles.x = Mathf.Clamp(rotationAngles.x, -Mathf.Abs(maxHorizontalOffset), Mathf.Abs(maxHorizontalOffset));
        rotationAngles.y = Mathf.Clamp(rotationAngles.y, -Mathf.Abs(maxVerticalOffset), Mathf.Abs(maxVerticalOffset));
        Vector3 recoilOffset = new Vector3(0, 0, GetRecoilMagnitude() * maxRecoilOffset);

        Vector3 targetArmsPosition = armsHolderPivot - new Vector3(rotationAngles.x, rotationAngles.y, 0) - walkingArmOffsetMagnitude * new Vector3(walkingInput.x, 0, walkingInput.y) - recoilOffset;

        // Landing effect
        if (!wasGrounded && player.IsNearGround())
        {
            currentLandingArmsAngle = landingArmsAngle;
            PlayFootstepSound();
            PlayFootstepSound();
        }

        // In air
        if (!player.IsNearGround())
            targetArmRotation.eulerAngles = new Vector3(targetArmRotation.eulerAngles.x + inAirArmsAngle, targetArmRotation.eulerAngles.y, targetArmRotation.eulerAngles.z);
        else
        {
            targetArmRotation.eulerAngles = new Vector3(targetArmRotation.eulerAngles.x + currentLandingArmsAngle, targetArmRotation.eulerAngles.y, targetArmRotation.eulerAngles.z);
            currentLandingArmsAngle = Mathf.Lerp(currentLandingArmsAngle, 0, Time.deltaTime * 10);
        }

        // Attack director adjustments
        Vector3 attackDirectorRotation = attackDirector.localRotation.eulerAngles;
        targetArmRotation.eulerAngles += attackDirectorRotation;

        // Applying effect
        armsHolder.localRotation = Quaternion.Slerp(armsHolder.localRotation, targetArmRotation, Time.deltaTime * walkingLerpParameter);
        armsHolder.localPosition = Vector3.Lerp(armsHolder.localPosition, targetArmsPosition, Time.deltaTime * 10);
        cameraHolder.localRotation = Quaternion.Slerp(cameraHolder.localRotation, Quaternion.Euler(sprintCameraMultiplier * walkingHeadBobMagnitude * walkingMagnitude * new Vector3(Mathf.Abs(Mathf.Cos(walkingParameter)), 0, 0)), Time.deltaTime * walkingLerpParameter);

        wasGrounded = player.IsNearGround();

    }

    private bool IsSprinting()
    {
        return player.IsSprinting && player.IsNearGround();
    }

    private bool IsReloading()
    {
        return player.PerformingAction && player.CurrentActionType == PlayerCharacter.ActionType.Reload;
    }

    private bool IsFiring()
    {
        return player.PerformingAction && player.CurrentActionType == PlayerCharacter.ActionType.Fire;
    }

    private float GetRecoilMagnitude()
    {
        return player.GetRecoilMagnitude();
    }



    #endregion


    #region Audio

    public void PlayWeaponSound(int soundIndex)
    {
        if (soundIndex < 0 || soundIndex == currentSet.miscSounds.Length)
            return;

        Audio.PlaySFX(currentSet.miscSounds[soundIndex], transform.position + transform.forward, transform, 1f);
    }

    public void PlayFireSound()
    {
        Audio.PlaySFX(currentSet.fireSound, transform.position + transform.forward, transform, 1f);
    }

    private void PlayFootstepSound()
    {
        Audio.PlaySFX(footstepSound, player.Position, player.transform, 0.65f);
    }

    public void PlaySlideSound()
    {
        Audio.PlaySFX(slideSound, player.transform.position, player.transform, 1f);
    }


    public void PlayDamageSound()
    {
        Audio.PlaySFX(playerDamageSound, player.head.position, player.head, 0);
    }

    public void PlayPunchSoundEffect()
    {
        Audio.PlaySFX(punchSound, player.head.position, player.head, 0);
    }

    #endregion

    #region Dash

    public void DashEffectOn()
    {
        dashEffect.SetActive(true);
    }

    public void DashEffectOff()
    {
        dashEffect.SetActive(false);
    }

    #endregion

    #region Death

    public void Death()
    {
        SetTrigger(deathTrigger);
    }

    #endregion

    #region Ability

    public void SlamStart()
    {
        SetTrigger(slamStartTrigger);
    }

    public void SlamEnd()
    {
        SetTrigger(slamEndTrigger);
    }
    
    public void PlaySlamStartSound()
    {
        Audio.PlaySFX(slamStartSound, transform.position, transform, 0.8f);
    }

    public void PlaySlamEndSound()
    {
        Audio.PlaySFX(slamEndSound, transform.position, transform, 0.8f);
    }
    #endregion
}
