using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
// I just want to keep my States clean
// Needs Update to hinder player from abusing wall climb
// i have a lot of publics in my scripts because i haven't yet refactored these fields and or methods
// another reason is that i had many scripts that rely on the information of these fields but i created a lot of redundancies while working on the script
// AI additions are obvious i guess, otherwise the stamina system was worked out with Gemini due to time restraints
public class StateController : MonoBehaviour
{
    [Header("Reference")]
    public PlayerController pc;
    public Vector2 currentInput;
    public Rigidbody rb;
    public Transform cameraTransform;

    // The whole wallrun and climb abuse is now more intuitively solved by a stamina system. timers are just too easy to trick or too obscure to deal with
    // the stamina system doesn't even need to be visible to the player

    [Header("Stamina System")]
    [Range(0, 200)] public float maxStamina; // Max stamina capacity
    public float currentStamina; // Current stamina
    [Range(0, 20)] public float staminaRegenRate; // Stamina regenerated per second
    [Range(0, 100)] public float wallInteractionStaminaCost; // Stamina consumed per second while on wall

    [Header("CollisionChecks")]
    public LayerMask Ground;
    public LayerMask Wall;
    public LayerMask Scalable;
    // A custom LayerMask i use to interact with Ground and Scalable Objects to enable the player to move on crates, etc.
    public LayerMask combinedRunnableMasks; 

    public RaycastHit leftWallHit;
    public RaycastHit rightWallHit;
    public RaycastHit frontWallHit;

    // Circumstances made a spherecast check for GroundCheck neccessary
    public float groundSphereRadius;
    // PlayerHeight to Check Raycast Hit on Ground
    public float playerHeight;
    // to check which side the wall is closest to determine if wallrun or climb should be enabled
    public bool leftWallrunEnabled;
    public bool rightWallrunEnabled;
    public bool frontWallClimbEnabled;
    // to check if a wall is even hit
    public bool leftWallDetected;
    public bool rightWallDetected;
    public bool frontWallDetected;
    // Helper property to interrupt infinite Wallrunning
    public bool anyWallDetected => leftWallDetected || rightWallDetected || frontWallDetected;
    // Angle thresholds for wall types // AI that wasn't a bad solution after all, tired
    [Header("Wall Type Detection")]
    [Range(0, 180)] public float minWallRunAngle; // Angle between player forward and wall normal
    [Range(0, 180)] public float maxWallRunAngle; // Perfect wall run is 90 degrees
    [Range(0, 180)] public float minClimbAngle;    // Angle between player forward and wall normal 
    [Range(0, 180)] public float maxClimbAngle;   // Perfect climb is 0 degrees (straight wall), // if you rotate 180 degress (its 180 deg)

    public float wallCheckDistance;
    public float minJumpHeight;
    public bool isGrounded;

    [Header("Cooldowns")]
    // common Timer needed to not make climb and wallrun combo abusable
    [Range(0, 5f)] public float wallRunClimbCooldown;
    private float wallRunClimbCooldownTimer;
    public bool IsOnWallCooldown;

    // States
    private State currentState;

    public IdleState idleState;
    public GroundRunning groundRunning;
    public WallRunning wallRunning;
    public JumpFromGround jumpingFromGround;
    public JumpFromWall jumpingFromWall;
    public ClimbWall climbWall;
    public AirBourne airBourne;

    // Consider Interaction state to pin player in place but i think it's unnecessary for now
    // Maybe if i create Interactable Obstacles for more parcour it will need to lock the player out for the animation to play out
    // depending on the Interactable

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // saftey check
        if(pc == null)
            //Finds the playercontroller and thus the player automatically
            pc = FindAnyObjectByType<PlayerController>();
        rb = pc.rb;
        cameraTransform = pc.cameraTransform;
        //Instantiate the States to ensure they are ready throughout the LifeCycle
        idleState = new IdleState();
        groundRunning = new GroundRunning();
        wallRunning = new WallRunning();
        jumpingFromGround = new JumpFromGround();
        jumpingFromWall = new JumpFromWall();
        climbWall = new ClimbWall();
        airBourne = new AirBourne();

        currentStamina = maxStamina; // Initialize stamina
        combinedRunnableMasks = Ground | Scalable;
        ChangeState(idleState);

        // Default Settings if i forget to set values
        if(maxStamina == 0 || staminaRegenRate == 0 || wallInteractionStaminaCost == 0)
        {
            maxStamina = 100;
            staminaRegenRate = 10;
            wallInteractionStaminaCost = 20;
        }
    }
    // Update is called once per frame
    void Update()
    {
        // Stamina Consumption for Wall Running/Climbing
        if (currentState == wallRunning || currentState == climbWall)
        {
            currentStamina -= wallInteractionStaminaCost * Time.deltaTime;
            currentStamina = Mathf.Max(0, currentStamina); // Ensure stamina doesn't go below 0
            // Removed: Decrement the specific wall run/climb duration timer as a secondary limit
        }
        // Stamina Regeneration for other states (Idle, GroundRunning, AirBourne)
        else if (currentState == idleState || currentState == groundRunning || currentState == airBourne)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(maxStamina, currentStamina); // Ensure stamina doesn't exceed maxStamina
        }

        CheckState();
        currentState?.OnStateUpdate();
        WallRunClimbCooldown();

        // Force player off wall if stamina runs out
        if ((currentState == wallRunning || currentState == climbWall) && currentStamina <= 0 && !IsOnWallCooldown)
        {
            ChangeState(jumpingFromWall); // Changed from airBourne to jumpingFromWall to force the player away
            return; // Important: return after changing state to prevent immediate re-evaluation in CheckState this frame.
        }
    }
    public void FixedUpdate()
    {
        currentState?.OnStateFixedUpdate();
    }
    #region Statechecks
    private void CheckState()
    {
        GroundCheck();
        AboveGround();
        WallCheck();

        // New condition: Player must have stamina to enter wall-related states
        bool hasStaminaForWallInteraction = currentStamina > 0;

        // Just to force the player from the wall if they stick to it too long for my taste, i will add a froce that pushes them off the wall
        // so this is another redundancy in my eyes
        if((currentState == wallRunning || currentState == climbWall) && currentStamina <= 0)
        {
            ChangeState(jumpingFromWall);
            return;
        }
        // JumpFromWall
        else if ((leftWallrunEnabled || rightWallrunEnabled || frontWallClimbEnabled) && pc.jumpPressed && AboveGround() )
        {
            ChangeState(jumpingFromWall);
        }
        // Wallrunning
        else if ((leftWallrunEnabled || rightWallrunEnabled) && pc.climbPressed &&  AboveGround() && !IsOnWallCooldown && hasStaminaForWallInteraction)// maybe i should move more things from playerController in here
        {
            ChangeState(wallRunning);
        }
        // Climbing, i think i can use this in other Interactables or better Scalable objects, as soon as i create them
        else if (frontWallClimbEnabled && pc.climbPressed && !leftWallrunEnabled && !rightWallrunEnabled && !IsOnWallCooldown && hasStaminaForWallInteraction)
        {
            ChangeState(climbWall);
        }
        // JumpFromGround
        else if (pc.jumpPressed && isGrounded)
        {
            ChangeState(jumpingFromGround);
        }
        // Running, walking seemed redundant and useless, unless stamina becomes a thing...
        else if (isGrounded && currentInput != Vector2.zero)
        {
            ChangeState(groundRunning);
        }
        // Idle
        else if (isGrounded && currentInput == Vector2.zero)
        {
            ChangeState(idleState);
        }
        // Fallback if everything else doesn't apply, hope the player will hit some ground at some point, or traps, or lava, who knows, i need traps
        else
        {
            ChangeState(airBourne);
        }

    }
    #endregion
    #region Checks for Statecheck
    public LayerMask CombinedLayerMasks()
    {
        return combinedRunnableMasks;
    }
    private bool GroundCheck()
    {
        Vector3 sphereOrigin = rb.transform.position + Vector3.up * groundSphereRadius;
        RaycastHit hit ;
        isGrounded = Physics.SphereCast(sphereOrigin, groundSphereRadius, Vector3.down, out hit, playerHeight * 0.6f, combinedRunnableMasks); 
        return isGrounded ;
    }
    private void WallCheck()
    {
        // Reset values for new checks
        leftWallrunEnabled = false;
        rightWallrunEnabled = false;
        frontWallClimbEnabled = false;
        leftWallDetected = false;
        rightWallDetected = false;
        frontWallDetected = false;

        // LeftWallCheck
        if (Physics.Raycast(cameraTransform.transform.position, -cameraTransform.transform.right, out leftWallHit, wallCheckDistance, Wall))
        {
            leftWallDetected = true;
            float angle = Vector3.Angle(cameraTransform.forward, leftWallHit.normal);
            if (angle >= minWallRunAngle && angle <= maxWallRunAngle)
                leftWallrunEnabled = true;
        }
        // RighWallCheck
        if (Physics.Raycast(cameraTransform.transform.position, cameraTransform.transform.right, out rightWallHit, wallCheckDistance, Wall))
        {
            rightWallDetected = true;
            float angle = Vector3.Angle(cameraTransform.forward, rightWallHit.normal);
            if(angle >= minWallRunAngle && angle <= maxWallRunAngle)
                rightWallrunEnabled = true;
        }
        // FrontWallCheck
        if (Physics.Raycast(cameraTransform.transform.position, cameraTransform.transform.forward, out frontWallHit, wallCheckDistance, Wall))
        {
            frontWallDetected = true;
            float angle = Vector3.Angle(cameraTransform.forward, frontWallHit.normal);
            if(angle >= minClimbAngle && angle <= maxClimbAngle)
                frontWallClimbEnabled = true;
        }
    }
    private bool AboveGround()
    {
        return !Physics.Raycast(rb.transform.position, Vector3.down, minJumpHeight, Ground);
    }
    public void WallRunClimbCooldown()
    {
        if (IsOnWallCooldown)
        {
            wallRunClimbCooldownTimer -= Time.deltaTime;
            if( wallRunClimbCooldownTimer <= 0 )
            {
                IsOnWallCooldown = false;
            }
        }
    }
#endregion
    public void ChangeState(State newState)
    {
        if(newState == currentState) return;

        State previousState = currentState;
        bool wasInWallState = (previousState == wallRunning || previousState == climbWall);
        bool entersWallState = (newState == wallRunning || newState == climbWall);
        bool entersJumpFromWall = (newState == jumpingFromWall);

        if ((wasInWallState || (previousState == jumpingFromWall)) && (!entersWallState && !anyWallDetected) || entersJumpFromWall)
        {
            wallRunClimbCooldownTimer = wallRunClimbCooldown;
            IsOnWallCooldown = true;
        }

        previousState?.OnStateExit();
        currentState = newState;
        currentState.OnStateEnter(this, pc);
    }
    public void SetMoveInput(Vector2 moveInput)
    {
        currentInput = moveInput;
    }
}
