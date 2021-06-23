using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Character : Entity {
    public Transform head;

    [Header("Collider")]
    public Vector3 center = Vector3.up;
    [Range(0,10)]
    public float radius = 0.5f;
    [Range(0, 10)]
    public float height = 2;
    protected CharacterController controller;
    private NavMeshAgent navAgent;
    private CapsuleCollider capsuleCollider;
    private bool useNavAgent = true;

    [Header("Movement")]
    protected Vector3 verticalVelocity;
    private Vector3 inertialMovementVector = Vector3.zero;
    private Vector3 inertialSmoothVelocity = Vector3.zero;
    private Vector3 movementVector;
    //private float trueMoveSpeed;

    private Vector3 manualMovementVector = Vector3.zero;
    private Vector3 inAirMovementVector = Vector3.zero;

    // Action
    private float actionTimeCounter = 0;
    private bool actionFlag = false;
    private Utility.VoidFunction actionEndCallback = null;

    public bool PerformingAction { get { return actionTimeCounter > 0 || actionFlag; } }
    public enum ActionType { Fire, Reload, Misc };
    public ActionType CurrentActionType { get; private set; }

    void Start()
    {
        controller = gameObject.AddComponent<CharacterController>();
        controller.center = center;
        controller.radius = radius;
        controller.height = height;

        capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.center = center;
        capsuleCollider.radius = radius;
        capsuleCollider.height = height;

        navAgent = gameObject.AddComponent<NavMeshAgent>();
        navAgent.radius = radius;
        navAgent.height = height;
        navAgent.speed = GetCharacterStats().moveSpeed;
        navAgent.acceleration = 1000;
        navAgent.angularSpeed = 1440;
        navAgent.enabled = true;//false

        ChangeMovementMode(true);

        //path = new NavMeshPath();

        ControllerStart();
    }

    void Update()
    {
        ActionTimeCounting();
        ControllerUpdate();
    }

    protected abstract void ControllerStart();

    protected abstract void ControllerUpdate();

    public abstract CharacterStats GetCharacterStats();

    #region Public Movement
    private void ChangeMovementMode(bool useNavAgent)
    {
        this.useNavAgent = useNavAgent;
        if(navAgent)
            navAgent.enabled = useNavAgent;
        if(capsuleCollider)
            capsuleCollider.enabled = useNavAgent;
        if(controller)
            controller.enabled = !useNavAgent;
    }

    /// <summary>
    /// Returns character's move speed
    /// </summary>
    /// <returns></returns>
    public abstract float GetMoveSpeed();

    /// <summary>
    /// Returns character's in air move speed
    /// </summary>
    /// <returns></returns>
    public abstract float GetInAirSpeed();

    /// <summary>
    /// Tells character to move to a given position
    /// </summary>
    /// <param name="position"></param>
    public void MoveToPosition(Vector3 position)
    {
        SetDestination(position, 0);
        //UpdatePath(position);
        //FollowPath(0);
    }

    /// <summary>
    /// Tells character to follow the position with stopping distance
    /// </summary>
    /// <param name="positionToFollow"></param>
    /// <param name="stoppingDistance"></param>
    /// <param name="lookAtPosition"></param>
    public void Follow(Vector3 positionToFollow, float stoppingDistance, bool lookAtPosition = true)
    {
        //UpdatePath(positionToFollow);
        //FollowPath(stoppingDistance);
        SetDestination(positionToFollow, stoppingDistance);

        if (Vector3.Distance(positionToFollow, transform.position) > stoppingDistance)
            return;
        if (lookAtPosition)
            LookAt(positionToFollow, 1);
    }

    public void StopMovement()
    {
        if(navAgent.isOnNavMesh)
            navAgent.ResetPath();
    }

    private void SetDestination(Vector3 position, float stoppingDistance)
    {
        ChangeMovementMode(true);
        navAgent.SetDestination(position);
        navAgent.speed = GetCharacterStats().moveSpeed;
        navAgent.stoppingDistance = stoppingDistance;
    }

    /// <summary>
    /// Inertial movement with custom speed relative to the given direction that character is facing 
    /// </summary>
    /// <param name="moveInput"></param>
    /// <param name="forward"></param>
    /// <param name="right"></param>
    /// <param name="speed"></param>
    /// <param name="inertiaModifier"></param>
    public void ManualMovementCustomSpeed(Vector2 moveInput, Vector3 forward, Vector3 right, float speed)
    {
        ChangeMovementMode(false);
        if (moveInput.magnitude > 1)
            moveInput.Normalize();

        Vector3 moveVector3 = forward * moveInput.y + right * moveInput.x;

        Vector3 groundNormal = GetGroundNormal();
        // Slope correction
        if (groundNormal != Vector3.up)
            moveVector3 = moveVector3.magnitude * (moveVector3 - Vector3.Dot(moveVector3, groundNormal) * groundNormal / groundNormal.sqrMagnitude).normalized;
        
        if (IsNearGround())
        {
            manualMovementVector = Vector3.Lerp(manualMovementVector, moveVector3, Time.deltaTime * 10);
            inAirMovementVector = Vector3.zero;
        }
        else
        {
            inAirMovementVector = Vector3.Lerp(inAirMovementVector, moveVector3, Time.deltaTime * 10);
            NonInertialMovement(GetInAirSpeed() * inAirMovementVector);
        }

        InertialMovement(manualMovementVector, speed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + manualMovementVector);
    }

    /// <summary>
    /// Inertial movement relative to the given direction that character is facing 
    /// </summary>
    /// <param name="moveInput"></param>
    /// <param name="forward"></param>
    /// <param name="right"></param>
    /// <param name="inAirMoveSpeedMultiplier"></param>
    /// <param name="inertiaModifier"></param>
    public void ManualMovement(Vector2 moveInput,Vector3 forward,Vector3 right)
    {
        ManualMovementCustomSpeed(moveInput, forward, right, GetMoveSpeed());
    }

    #endregion

    #region Physical Movement

    /// <summary>
    /// Returns true if character is near ground
    /// </summary>
    /// <returns></returns>
    public bool IsNearGround()
    {
        return Physics.Raycast(new Ray(transform.position, Vector3.down), 0.3f) || controller.isGrounded;
    }

    /// <summary>
    /// Applies inertial movement to the movement vector
    /// </summary>
    /// <param name="moveDirection"></param>
    /// <param name="checkGround"></param>
    /// <param name="multiplier"></param>
    /// <param name="smoothTime"></param>
    protected void InertialMovement(Vector3 moveDirection,float moveSpeed, bool checkGround = true, float multiplier = 1, float smoothTime = 0.025f)
    {
        if (controller.isGrounded || IsNearGround() || !checkGround)
        {
            moveDirection *= Time.deltaTime * moveSpeed * multiplier;

            float velocityMultiplier = GetControllerVelocityCoefficient(moveSpeed);
            float velocityMultiplierStrength = 0.7f;

            inertialMovementVector = Vector3.SmoothDamp(inertialMovementVector, moveDirection * (1 + velocityMultiplierStrength * (velocityMultiplier-1)), ref inertialSmoothVelocity, smoothTime);
        }
        movementVector += inertialMovementVector;
        inertialMovementVector -= inertialMovementVector * Time.deltaTime * GetInertiaResistanceStrength();
    }

    protected abstract float GetInertiaResistanceStrength();

    public float GetControllerVelocityCoefficient(float moveSpeed)
    {
        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0;
        return Mathf.Clamp(horizontalVelocity.magnitude / moveSpeed, 0, 1);
    }

    /// <summary>
    /// Redirects inertia relative to the direction that character is facing
    /// </summary>
    /// <param name="moveInput"></param>
    /// <param name="forward"></param>
    /// <param name="right"></param>
    public void RedirectInertia(Vector3 newDirection)
    {
        if (newDirection.magnitude == 0)
            return;

        manualMovementVector = Vector3.zero;
        inertialMovementVector = newDirection.normalized * inertialMovementVector.magnitude;
    }

    /// <summary>
    /// Resets all inertia and movement
    /// </summary>
    public void ResetInertia()
    {
        inertialMovementVector = Vector3.zero;
        verticalVelocity = Vector3.zero;
        movementVector = Vector3.zero;
    }

    /// <summary>
    /// Sets vertical velocity
    /// </summary>
    /// <param name="val"></param>
    public void SetVerticalVelocity(float val)
    {
        verticalVelocity = Vector3.up * val;
    }

    /// <summary>
    /// Applies non-inertial movement to the movement vector
    /// </summary>
    /// <param name="moveSpeedVector"></param>
    public void NonInertialMovement(Vector3 moveSpeedVector)
    {
        moveSpeedVector *= Time.deltaTime;
        if (moveSpeedVector.magnitude == 0)
            return;

        movementVector += moveSpeedVector;
    }

    /// <summary>
    /// Applies gravity to the movement vector
    /// </summary>
    public void Gravity()
    {
        if (!IsNearGround() && useNavAgent)
            ChangeMovementMode(false);

        if (controller.isGrounded && verticalVelocity.y < 0)
            verticalVelocity = Vector3.zero;

        verticalVelocity += GetCharacterStats().gravityModifier * Physics.gravity * Time.deltaTime;
        Vector3 deltaPosition = verticalVelocity * Time.deltaTime;
        Vector3 move = Vector3.up * deltaPosition.y;

        movementVector += move;
    }

    /// <summary>
    /// Applies jump force
    /// </summary>
    public void Jump()
    {
        if (!controller.isGrounded)
            return;
        ChangeMovementMode(false);
        verticalVelocity.y = GetJumpVelocity();
    }

    public abstract float GetJumpVelocity();

    /// <summary>
    /// Applies the movement vector
    /// </summary>
    /// <param name="multiplier"></param>
    public void ApplyMovement(float multiplier = 1)
    {
        if (!useNavAgent)
        {
            controller.Move(movementVector * multiplier);
        }
        movementVector = Vector3.zero;
    }

    public Vector3 GetGroundNormal()
    {
        if (Physics.Raycast(new Ray(transform.position, Vector3.down), out RaycastHit hit, 1f))
            if (hit.normal != Vector3.up)
                return hit.normal;

        return Vector3.up;
    }

    public void Warp(Vector3 position)
    {
        //ResetInertia();
        controller.enabled = false;
        transform.position = position;
        navAgent.Warp(position);
        controller.enabled = true;
    }

    /// <summary>
    /// Same as calling CharacterController.Move(Vector3 motion);
    /// </summary>
    /// <param name="motion"></param>
    public void JustMove(Vector3 motion)
    {
        if (!controller)
            return;
        ChangeMovementMode(false);
        controller.Move(motion);
    }

    /// <summary>
    /// Destroys NavMeshAgent and CharacterController (Needed in DeadlySpikes for some reason)
    /// </summary>
    public void DestroyMovementComponents()
    {
        Destroy(navAgent);
        Destroy(capsuleCollider);
        Destroy(controller);
    }

    #endregion

    #region Rotation
    // Tells character to look at a target poisition
    public void LookAt(Vector3 target,float lerpParameter)
    {
        target.y = transform.position.y;
        Vector3 direction = target - transform.position;
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, direction,lerpParameter));
    }
    #endregion

    #region Attack

    public abstract Vector3 GetAttackSource();

    public abstract Vector3 GetAttackDirection();

    public virtual void DamageFeedback(Entity damagedEntity,DamageOutcome damageOutcome) { }

    protected override float GetSelfDamageMultiplier()
    {
        return GetCharacterStats().selfDamageMultiplier;
    }

    #endregion

    #region Action
    protected void Action(Utility.VoidFunction startCallback, Utility.VoidFunction endCallback, float duration, ActionType type)
    {
        if (PerformingAction)
            return;

        actionEndCallback = endCallback;
        startCallback();
        CurrentActionType = type;
        actionTimeCounter = duration;
        actionFlag = true;
    }

    void ActionTimeCounting()
    {
        if (actionTimeCounter == 0 && !actionFlag)
            return;

        actionTimeCounter -= Time.deltaTime;

        if (actionTimeCounter <= 0)
        {
            actionFlag = false;
            actionTimeCounter = 0;
            if (actionEndCallback != null)
            {
                Utility.VoidFunction func = actionEndCallback;
                actionEndCallback = null;
                func();
            }
        }
    }
    #endregion

    protected override float RecalculateRawDamage(float rawDamage, Entity damageGiver)
    {
        if (damageGiver == this)
            return rawDamage * GetCharacterStats().selfDamageMultiplier;
        return rawDamage;
    }
}
