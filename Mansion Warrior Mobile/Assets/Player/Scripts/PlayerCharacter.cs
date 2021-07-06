using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCharacter : Character
{
    public static PlayerCharacter Instance { get; private set; }
    public Transform attackDirector;

    [Header("References")]
    public PlayerStats stats;
    public PlayerInput input;
    public HUD hud;
    public PlayerAnimator animator;
    public Camera mainCamera;

    // State Machine
    private PlayerState currentState;

    // Movement
    public bool EnableGravity { get; set; }
    public bool IsCrouching { get; set; }


    [Header("Rotation")]
    public float maxVerticalRotationAngle = 90;
    public float VerticalAimAngle { get; private set; }
    public float HorizontalAimAngle { get; private set; }


    public bool IsSprinting { get; set; }
    // Slide
    public float SlideDuration { get; private set; }

    // Punch
    public bool IsMeleeing { get; set; }

    // Weapons
    private float currentRecoilParameter = 0;
    private float currentRecoilSpeed;
    private float currentRecoilDuration;
    private bool cancelReload;

    private PlayerInventory.EquippedWeapon weaponToEquip;

    public PlayerInventory PlayerInventory { get; private set; }

    // Interaction
    public Interactable CurrentInteractable { get; private set; }
    public Entity AutoTargetedEntity { get; private set; }
    public bool CanAutoTarget { get; set; }

    // Ability
    public float AbilityCharge { get; private set; }

    protected override void ControllerStart()
    {
        if (Instance == null)
            Instance = this;
        else
            TakeDamage(GetMaxHealth() / GetSelfDamageMultiplier(), this, transform.position);

        Utility.DisableCursor();

        SetStartingResources();

        EnableGravity = true;
        CanAutoTarget = true;
        InitializeStateMachine();
    }

    protected override void ControllerUpdate()
    {
        //////////
        if (gameFinish || IsDead)
            return;
 

        HUDUpdate();
        if (GameManager.IsPaused())
            return;

        SlideDurationRecovery();

        AbilityRefill();
        AutoTargetViewModelUpdate();
        UpdateInteractable();
        StateMachine();

        AutoTargetUpdate();
        CrouchUpdate();
        RecoilUpdate();
    }

    public override CharacterStats GetCharacterStats()
    {
        return stats;
    }

    #region Game Finish
    public bool gameFinish = false;
    public void FinishGame()
    {
        if (gameFinish)
            return;
        gameFinish = true;
        Utility.EnableCursor();
    }

    #endregion

    #region StateMachine

    private void InitializeStateMachine()
    {
        currentState = new PlayerStates.DefaultState();
        currentState.Init(this);
    }

    private void StateMachine()
    {
        currentState.Action(this);
        PlayerState nextState = currentState.Transition(this);
        if (nextState != null)
        {
            currentState = nextState;
            currentState.Init(this);
        }
    }

    #endregion

    #region Movement and Rotation

    public void Rotation(Vector2 rotationInput)
    {
        VerticalAimAngle = Mathf.Clamp(VerticalAimAngle - rotationInput.y, -Mathf.Abs(maxVerticalRotationAngle), Mathf.Abs(maxVerticalRotationAngle));

        Vector2 recoilAngles = GetRecoilAngles();
        head.localRotation = Quaternion.Euler(VerticalAimAngle - recoilAngles.y, recoilAngles.x, 0);

        HorizontalAimAngle += rotationInput.x;
        transform.Rotate(new Vector3(0, rotationInput.x, 0));
    }

    public void SetVerticalAimAngle(float newAngle)
    {
        VerticalAimAngle = Mathf.Clamp(newAngle, -Mathf.Abs(maxVerticalRotationAngle), Mathf.Abs(maxVerticalRotationAngle));
        Vector2 recoilAngles = GetRecoilAngles();
        head.localRotation = Quaternion.Euler(VerticalAimAngle - recoilAngles.y, recoilAngles.x, 0);
    }

    private void CrouchUpdate()
    {
        float controllerCenter = 1;
        float controllerHeight = 2;
        float headPosition = 1.75f;
        if (IsCrouching)
        {
            controllerCenter /= 2;
            controllerHeight /= 2;
            headPosition /= 2;
        }
        verticalTargetingOffset = controllerCenter;
        controller.center = Vector3.Lerp(controller.center, Vector3.up * controllerCenter, Time.deltaTime * 10);
        controller.height = Mathf.Lerp(controller.height, controllerHeight, Time.deltaTime * 10);
        head.localPosition = Vector3.Lerp(head.localPosition, Vector3.up * headPosition, Time.deltaTime * 10);
    }

    public override float GetMoveSpeed()
    {
        return stats.moveSpeed * (IsSprinting ? stats.sprintSpeedMultiplier : 1);
    }

    public override float GetInAirSpeed()
    {
        return stats.inAirSpeed;
    }

    protected override float GetInertiaResistanceStrength()
    {
        return stats.inertiaResistanceStrength;
    }

    public override float GetJumpVelocity()
    {
        return stats.jumpVelocity;
    }

    #endregion

    #region Auto Targetting

    private void AutoTargetUpdate()
    {
        if (!AutoTargetEnabled())
        {
            AutoTargetedEntity = null;
            return;
        }

        var foundEntities = from entity in EntityRegistry.GetInstance().GetClosestEntities(head.position, stats.autoTargetRange, this)
                            where Utility.IsVisible(head.position, entity.gameObject, stats.autoTargetRange, entity.Position,stats.autoTargetLayerMask) &&
                            Utility.WithinAngle(head.position, head.forward, entity.Position, stats.autoTargetAngle)
                            orderby Vector3.Distance(head.forward, (entity.Position - head.position).normalized)
                            select entity;

        if (foundEntities.Count() == 0)
            AutoTargetedEntity = null;
        else
            AutoTargetedEntity = foundEntities.First();
    }

    private void AutoTargetViewModelUpdate()
    {
        if (AutoTargetedEntity)
            attackDirector.rotation = Quaternion.Slerp(attackDirector.rotation, Quaternion.LookRotation(AutoTargetedEntity.Position - attackDirector.position), Time.deltaTime * 10);
        else
            attackDirector.localRotation = Quaternion.Slerp(animator.transform.localRotation, Quaternion.identity, Time.deltaTime * 10);
    }

    private bool AutoTargetEnabled()
    {
        return input.AutoTargetEnabled() && CanAutoTarget;
    }
    #endregion

    #region Fire Weapon
    public override Vector3 GetAttackSource()
    {
        return head.transform.position;
    }

    public override Vector3 GetAttackDirection()
    {
        Vector2 unitCircle = Random.insideUnitCircle * Mathf.Sin(Mathf.Deg2Rad * GetSpreadAngle() / 2);

        Transform forwardTransform = AutoTargetedEntity ? attackDirector : head;

        return forwardTransform.forward + forwardTransform.up * unitCircle.y + forwardTransform.right * unitCircle.x;
    }
    
    public void FireWeaponAction()
    {
        if (PerformingAction && CurrentActionType == ActionType.Reload)
            CancelReload();
        else if (GetCurrentWeaponLoadedAmmo() >= GetCurrentWeaponAmmoPerShot())
        {
            IsSprinting = false;
            Action(FireWeaponStartCallback, FireWeaponEndCallback, null, animator.GetCurrentSet().fireDuration, ActionType.Fire);
        }
        else if (GetCurrentWeaponAmmo() + PlayerInventory.CurrentWeapon.loadedAmmo > 0)
            ReloadWeaponAction();
        else
            ChangeToWeaponWithAmmo();
    }

    public bool IsFiring()
    {
        return PerformingAction && CurrentActionType == ActionType.Fire;
    }

    void FireWeaponStartCallback()
    {
        animator.FireWeapon();
        PlayerInventory.CurrentWeapon.Shoot(this);
        AddRecoil(PlayerInventory.CurrentWeapon.weapon.recoilStrength, Mathf.Abs(PlayerInventory.CurrentWeapon.weapon.recoilDuration));
    }

    void FireWeaponEndCallback()
    {
        if (GetCurrentWeaponAmmo() + PlayerInventory.CurrentWeapon.loadedAmmo == 0)
            ChangeToWeaponWithAmmo();
    }
    #endregion

    #region Inspect Weapon
    public void InspectWeaponAction()
    {
        if (IsSprinting)
            return;

        Action(animator.InspectWeapon, null, null, 0, ActionType.Misc);
    }

    #endregion

    #region Equip Weapon

    void EquipWeaponAction()
    {
        if (PlayerInventory.CurrentWeapon == null)
            Action(EquipWeaponCallback, null, null, animator.GetWeaponSet(weaponToEquip.weapon).equipDuration, ActionType.Reload);
        else
            Action(animator.Holster, EquipWeaponSecondAction, null, animator.GetCurrentSet().holsterDuration, ActionType.Reload);
    }

    void EquipWeaponSecondAction()
    {
        Action(EquipWeaponCallback, null, null, animator.GetWeaponSet(weaponToEquip.weapon).equipDuration, ActionType.Reload);
    }

    void EquipWeaponCallback()
    {
        animator.EquipWeapon(weaponToEquip.weapon);
        animator.Equip();
        PlayerInventory.EquipWeapon(weaponToEquip);
        weaponToEquip = null;
    }

    public void SwapWeapons()
    {
        if(PlayerInventory.HolsteredWeapon != null)
            EquipWeapon(PlayerInventory.HolsteredWeapon.weapon, PlayerInventory.HolsteredWeapon.loadedAmmo);
    }

    #endregion

    #region Reload Weapon

    public void ReloadWeaponAction()
    {
        if (PlayerInventory.CurrentWeapon.IsFullyLoaded())
        {
            //UIMessage(Localizer.Localize("CLIP_IS_FULL"));
            return;
        }

        if (GetCurrentWeaponAmmo() == 0)
        {
            //UIMessage(Localizer.Localize("NO_AMMO"));
            return;
        }

        Action(animator.ReloadInit, ReloadWeaponInitCallback, null, animator.GetCurrentSet().reloadInitDuration, ActionType.Reload);
    }

    void ReloadWeaponInitCallback()
    {
        Action(animator.ReloadWeapon, ReloadWeaponCallback, null, animator.GetCurrentSet().reloadDuration, ActionType.Reload);
    }

    void ReloadWeaponCallback()
    {
        if (PlayerInventory.CurrentWeapon.IsFullyLoaded() || cancelReload || GetCurrentWeaponAmmo() == 0)
        {
            cancelReload = false;
            Action(animator.ReloadEnd, null, null, animator.GetCurrentSet().reloadEndDuration, ActionType.Reload);
        }
        else
            ReloadWeaponInitCallback();
    }

    public void ReloadCurrentWeapon()
    {
        PlayerInventory.CurrentWeapon.Reload(PlayerInventory);
    }

    public void CancelReload()
    {
        cancelReload = true;
    }

    #endregion

    #region Melee

    public void MeleeAction()
    {
        if (PerformingAction && CurrentActionType == ActionType.Reload || CurrentActionType == ActionType.Misc)
            InteruptAction();
        Action(MeleeStartCallback, MeleeEndCallback, MeleeInteruptCallback, animator.punchDuration, ActionType.Fire);
    }

    private void MeleeStartCallback()
    {
        animator.UnequipWeapons();
        animator.Punch();
        weaponToEquip = PlayerInventory.CurrentWeapon;
        CameraShaker.Shake(transform.position, 0.5f);
    }

    private void MeleeEndCallback()
    {
        EquipWeaponSecondAction();
    }

    private void MeleeInteruptCallback()
    {
        animator.EquipWeapon(weaponToEquip.weapon);
    }

    public void MeleeDamageCall()
    {
        stats.meleeFunction.DoDamage(this, stats.meleeDamage);
    }
    #endregion

    #region Slide and Jump Strength Recovery

    public void ConsumeSlide()
    {
        SlideDuration = 0;
    }

    private void SlideDurationRecovery()
    {
        if (SlideDuration >= stats.slideDuration)
        {
            SlideDuration = stats.slideDuration;
        }
        SlideDuration += Time.deltaTime * stats.slideDuration / stats.slideDurationRecoveryTime;
    }

    #endregion

    #region Damage

    public override void DamageFeedback(Entity damagedEntity, DamageOutcome damageOutcome)
    {
        hud.hitmarker.StartEffect(damageOutcome == DamageOutcome.Death);
    }

    protected override void DeathEffect()
    {
        hud.Hide();
        GameMode.Instance.FailEnd();
        InteruptAction();
        //hud.UIMessage(Localizer.Localize("restartGame"), true);
        animator.DashEffectOff();
        animator.Death();
    }

    protected override void OnDamageTaken(float rawDamage, Entity damageGiver, Vector3 sourcePosition)
    {
        CameraShaker.Shake(transform.position, 0.25f);
        animator.PlayDamageSound();
        hud.DamageIndicator(sourcePosition, this);
        GameMode.Instance.OnPlayerDamaged();
    }

    #endregion

    #region Recoil and Spread

    void AddRecoil(float recoilSpeed, float duration)
    {
        currentRecoilSpeed = recoilSpeed;
        currentRecoilDuration = duration;
    }

    void RecoilUpdate()
    {
        if (currentRecoilDuration == 0)
        {
            currentRecoilParameter = Mathf.Lerp(currentRecoilParameter, 0, Time.deltaTime * stats.recoilRecoveryParameter);
            return;
        }

        currentRecoilDuration -= Time.deltaTime;

        currentRecoilParameter += currentRecoilSpeed * Time.deltaTime;

        if (currentRecoilParameter > Mathf.Abs(stats.maxRecoilParameter))
            currentRecoilParameter = Mathf.Abs(stats.maxRecoilParameter);

        if (currentRecoilDuration < 0)
            currentRecoilDuration = 0;
    }

    public Vector2 GetRecoilAngles()
    {
        if (PlayerInventory.CurrentWeapon == null)
            return Vector2.zero;

        float radius = Random.Range(0, PlayerInventory.CurrentWeapon.weapon.recoilAngle * currentRecoilParameter / stats.maxRecoilParameter);
        float angle = Random.Range(0, Mathf.PI);

        return new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
    }

    public float GetRecoilMagnitude()
    {
        return currentRecoilParameter / stats.maxRecoilParameter;
    }

    public float GetSpreadAngle()
    {
        if (PlayerInventory.CurrentWeapon == null)
            return 0;

        return (1 + 
            (PlayerInventory.CurrentWeapon.weapon.spreadRecoilMultiplier * currentRecoilParameter / stats.maxRecoilParameter)) * PlayerInventory.CurrentWeapon.weapon.spreadAngle;
    }

    #endregion

    #region Inventory and Resources
    private bool ChangeToWeaponWithAmmo()
    {
        if(PlayerInventory.HolsteredWeapon != null && ( PlayerInventory.HolsteredWeapon.loadedAmmo > 0 || PlayerInventory.GetAmmoCount(PlayerInventory.HolsteredWeapon.weapon.ammoNameKey) > 0))
        {
            SwapWeapons();
            return true;
        }
        
        
        //int weaponSlot = PlayerInventory.GetWeaponWithAmmo();
        //if (weaponSlot >= 0)
        //{
        //    input.SetSelectedWeaponSlot(weaponSlot);
        //    return true;
        //}
        //else
        //    return false;

        return false;
    }

    private void SetStartingResources()
    {
        PlayerInventory = new PlayerInventory(this);
        PlayerInventory.AddAmmo(stats.startingAmmo);

        if (stats.secondaryWeapon)
            PlayerInventory.EquipWeapon(stats.secondaryWeapon, stats.secondaryWeapon.magazineSize);
        if(stats.primaryWeapon)
            EquipWeapon(stats.primaryWeapon, stats.primaryWeapon.magazineSize);
    }

    public override float GetMaxHealth()
    {
        return stats.maxHealth;
    }

    public bool HasWeapon(Weapon weapon)
    {
        return PlayerInventory.HasWeapon(weapon);
    }

    public void EquipWeapon(Weapon weapon, uint loadedAmmo)
    {
        if (weapon == null || PlayerInventory.CurrentWeapon != null && PlayerInventory.CurrentWeapon.weapon == weapon)
            return;
        //PlayerInventory.EquipWeapon(weapon, loadedAmmo);
        weaponToEquip = new PlayerInventory.EquippedWeapon(weapon, loadedAmmo);
        EquipWeaponAction();
    }

    uint GetCurrentWeaponAmmo()
    {
        if (PlayerInventory.CurrentWeapon == null)
            return 0;

        return PlayerInventory.GetAmmoCount(PlayerInventory.CurrentWeapon.weapon.ammoNameKey);
    }

    uint GetCurrentWeaponLoadedAmmo()
    {
        if (PlayerInventory.CurrentWeapon == null)
            return 0;

        return PlayerInventory.CurrentWeapon.loadedAmmo;
    }

    uint GetCurrentWeaponAmmoPerShot()
    {
        if (PlayerInventory.CurrentWeapon == null)
            return 0;
        return PlayerInventory.CurrentWeapon.weapon.ammoPerShot;
    }

    uint GetCurrentWeaponAmmoCap()
    {
        if (PlayerInventory.CurrentWeapon == null)
            return 0;

        return stats.GetAmmoCap(PlayerInventory.CurrentWeapon.weapon.ammoNameKey);
    }


    public void AddAmmo(string ammoNameKey, uint count)
    {
        PlayerInventory.AddAmmo(ammoNameKey, count);
        if (GetCurrentWeaponAmmo() + PlayerInventory.CurrentWeapon.loadedAmmo == 0)
            ChangeToWeaponWithAmmo();
    }

    public bool CanAddAmmo(string ammoNameKey)
    {
        return PlayerInventory.CanAddAmmo(ammoNameKey);
    }
    #endregion

    #region Interaction
    /// <summary>
    /// Tells character to interact with a current interactable
    /// </summary>
    public void Interact()
    {
        if (!CurrentInteractable)
            return;

        CurrentInteractable.Interact(this);
    }

    private void UpdateInteractable()
    {
        IEnumerable<Interactable> closest = InteractableRegistry.GetInstance().GetClosestInteractables(Position, stats.interactionDistance);

        var inter = from interactable in closest
                    where Utility.WithinAngle(head.position, head.forward, interactable.ButtonPosition, stats.interactionAngle)
                    && Utility.IsVisible(head.position, interactable.gameObject, stats.interactionDistance + 1, interactable.ButtonPosition)
                    select new { interactable, direcionalDistance = Vector3.Distance(head.forward, (interactable.ButtonPosition - head.position).normalized) };

        var temp = from x in inter
                   where x.direcionalDistance == inter.Min(y => y.direcionalDistance)
                   select x.interactable;

        CurrentInteractable = temp.Count() > 0 ? temp.First() : null;
    }
    #endregion

    #region Ability 

    private void AbilityRefill()
    {
        AbilityCharge += Time.deltaTime * stats.abilityChargeFillSpeed;
        if (AbilityCharge > stats.abilityMaxCharge)
            AbilityCharge = stats.abilityMaxCharge;
    }

    private void ConsumeAbility()
    {
        --AbilityCharge;
    }

    public bool CanUseAbility()
    {
        return AbilityCharge >= 1;
    }

    public void DoSlamDamage()
    {
        animator.PlaySlamEndSound();
        ConsumeAbility();
        Damage.ExplosiveDamage(Position, stats.slamDamage, stats.slamRange, this, stats.slamLayerMask, false);
        CameraShaker.Shake(Position, 1);
        if (stats.slamExplosionEffect)
            Instantiate(stats.slamExplosionEffect, Position, Quaternion.identity);
    }
    #endregion

    #region UI

    void HUDUpdate()
    {
        if (GameManager.IsPaused() || IsDead)
        {
            hud.Hide();
            return;
        }
        hud.Show();

        hud.reticle.SetSpreadAngle(GetSpreadAngle());
        hud.reticle.UpdateTarget(AutoTargetedEntity);

        hud.ammo.SetValues(GetCurrentWeaponAmmo(),GetCurrentWeaponLoadedAmmo());
        hud.healthbar.SetHealth(CurrentHealth, GetMaxHealth());
        hud.interactable.UpdatePrompt(this);
        hud.ability.UpdateGraphic(this);
    }


    public void PickupIndicator(Color indicatorColor)
    {
        hud.pickupIndicator.StartEffect(indicatorColor);
    }
    #endregion
}
