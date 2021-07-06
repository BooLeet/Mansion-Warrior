using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AIStates;

public class AICharacter : Character
{
    public AIStats stats;
    public AIAnimator animator;
    private AIDirector director;

    public bool HasAttackToken { get; private set; }
    public bool PlayerIsInfront { get; private set; }
    public bool PlayerIsVisible { get; private set; }
    public float DistanceToPlayer { get; private set; }
    public bool IsAlert { get; private set; }
    public bool CanAttack { get; private set; } = true;
    public bool PlayerIsDead { get { return player == null ? true : player.IsDead; } }

    public bool AttackOnCooldown { get { return !attackCooledDown; } }

    public Vector3 LastDamageSource { get; private set; }
    public bool PatrolCheckDamageDirection { get; set; }

    private PlayerCharacter player;
    public Vector3 LastSeenPlayerPosition { get; private set; }

    private float attackTokenCooldown;
    private bool attackCooledDown = true;
    private uint attacksPerTokenLeft;
    private Vector3 attackPlayerPosition;

    private AIState currentState;
    public string currentStateName;
    private bool dropGuaranteed = false;

    protected override void ControllerStart()
    {
        currentState = new PatrolState();
        director = AIDirector.GetInstance();
        director.Register(this);
        if (stats.alwaysAlert)
            IsAlert = true;
    }

    protected override void ControllerUpdate()
    {
        UpdatePlayerReference();
        PropertiesUpdate();
        AttackTokenCooldownUpdate();

        StateMachine();
        Gravity();
        ApplyMovement();
    }

    private void StateMachine()
    {
        if (IsDead)
            return;

        currentState.Action(this);
        AIState nextState = currentState.Transition(this);
        if (nextState != null)
        {
            currentState = nextState;
            currentState.Init(this);
        }
        currentStateName = currentState.ToString();
    }

    private void PropertiesUpdate()
    {
        if (player)
        {
            Vector3 coneStart = head.position - head.forward * 2;

            PlayerIsVisible = Utility.IsVisible(head.position, player.gameObject, stats.visibilityDistance, player.Position, stats.visibleLayers);

            PlayerIsInfront = Utility.WithinAngle(coneStart, head.forward, player.gameObject.transform.position + Vector3.up * player.verticalTargetingOffset, stats.visibilityAngle)
                          && Utility.WithinAngle(head.position, head.forward, player.gameObject.transform.position + Vector3.up * player.verticalTargetingOffset, 180)
                          && Utility.IsVisible(head.position, player.gameObject, stats.visibilityDistance, player.Position, stats.visibleLayers);
            DistanceToPlayer = Vector3.Distance(Position, player.Position);
        }
        else
        {
            PlayerIsVisible = false;
            PlayerIsInfront = false;
            DistanceToPlayer = 0;
        }

        if (PlayerIsVisible || stats.alwaysAlert)
            LastSeenPlayerPosition = player.Position;
    }

    private void UpdatePlayerReference()
    {
        if (player != null)
            return;

        player = PlayerCharacter.Instance;
    }

    public void Alarm(Vector3 playerPosition)
    {
        IsAlert = true;
        LastSeenPlayerPosition = playerPosition;
        director.Alarm(Position, playerPosition);
    }

    #region Attack Tokens
    private void AttackTokenCooldownUpdate()
    {
        if (attackTokenCooldown > 0)
            attackTokenCooldown -= Time.deltaTime;
        else if (!attackCooledDown)
        {
            attackCooledDown = true;
            attackTokenCooldown = 0;
            ReturnAttackToken();
        }
    }

    public void RequestAttackToken()
    {
        if (HasAttackToken)
            return;
        if (attackTokenCooldown == 0)
        {
            attacksPerTokenLeft = stats.attacksPerToken;
            HasAttackToken = stats.useAttackTokens ? director.RequestAttackToken(this) : true;
        }
    }

    public void ReturnAttackToken()
    {
        if (!HasAttackToken)
            return;
        HasAttackToken = false;
        if(stats.useAttackTokens)
            director.ReturnAttackToken(this);
    }

    private void SpendAttackToken()
    {
        --attacksPerTokenLeft;
        if (attacksPerTokenLeft == 0)
        {
            attackCooledDown = false;
            attackTokenCooldown = stats.attackTokenCooldownTime + Random.Range(0, 0.5f);
        }

    }

    #endregion

    #region Attack
    public void Attack()
    {
        Action(AttackStartCallback, AttackEndCallback, null, animator.attackAnimation.duration, ActionType.Fire);
    }

    void AttackStartCallback()
    {
        CanAttack = false;
        animator.Attack();
        SpendAttackToken();
        attackPlayerPosition = player.Position;
    }

    void AttackEndCallback()
    {
        CanAttack = true;
    }

    public void AttackDamageCall()
    {
        if(stats.damageFunction)
            stats.damageFunction.DoDamage(this, stats.attackDamage);
    }

    #endregion

    public void LookAtPlayer(float lerpParameter = 1)
    {
        if (player)
            LookAt(player.Position, lerpParameter);
    }

    public void Remove()
    {
        EntityRegistry.GetInstance().Unregister(this);
        director.Unregister(this);
        Destroy(gameObject);
    }

    #region Overrides

    public override void DamageFeedback(Entity damagedEntity, DamageOutcome damageOutcome)
    {
        
    }

    protected override void DeathEffect()
    {
        GameMode.Instance.OnEnemyKilled(this);
        DropLoot();
        ReturnAttackToken();
        director.Unregister(this);
        animator.Death(LastDamageSource);
        Destroy(gameObject);
    }

    public override Vector3 GetAttackDirection()
    {
        return (attackPlayerPosition - GetAttackSource()).normalized;
    }

    public override Vector3 GetAttackSource()
    {
        return animator.GetAttackSource();
    }

    public override CharacterStats GetCharacterStats()
    {
        return stats;
    }

    public override float GetInAirSpeed()
    {
        return 0;
    }

    protected override float GetInertiaResistanceStrength()
    {
        return 1;
    }

    public override float GetJumpVelocity()
    {
        return stats.jumpVelocity;
    }

    public override float GetMaxHealth()
    {
        return stats.maxHealth;
    }

    public override float GetMoveSpeed()
    {
        return stats.moveSpeed;
    }

    protected override float GetSelfDamageMultiplier()
    {
        return stats.selfDamageMultiplier;
    }

    protected override void OnDamageTaken(float rawDamage, Entity damageGiver, Vector3 sourcePosition)
    {
        animator.DamageEffect();
        LastDamageSource = sourcePosition;
        GameMode.Instance.OnEnemyDamaged();
        if (!IsAlert)
            PatrolCheckDamageDirection = true;
    }

    #endregion

    #region Loot

    public void GuaranteeDrop()
    {
        dropGuaranteed = true;
    }

    private void DropLoot()
    {
        if (dropGuaranteed)
        {
            InstantiateLoot();
            return;
        }

        if (stats.loot.Length == 0)
            return;

        if (Random.Range(0f, 1f) > stats.lootDropChance)
            return;

        InstantiateLoot();
    }

    private void InstantiateLoot()
    {
        ResourcePickup pickup = Instantiate(stats.GetRandomLoot(), transform.position, Quaternion.identity).GetComponentInChildren<ResourcePickup>();
        if (pickup)
            pickup.DestroyOnPickup = true;
    }

    #endregion
}
