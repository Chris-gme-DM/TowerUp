using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
// I just want to keep my States clean
public class StateController : MonoBehaviour
{
    [Header("Reference")]
    public PlayerController pc;
    public Vector2 currentInput;
    public Rigidbody rb;
    public Transform cameraTransform;

    [Header("CollisionChecks")]
    public LayerMask Ground;
    public LayerMask Wall;
    public LayerMask Scalable;

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
    [Range(0, 5f)] public float maxWallRunTime;
    public float wallRunTimer;


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

        combinedRunnableMasks = Ground | Wall | Scalable;
        ChangeState(idleState);
    }
    // Update is called once per frame
    void Update()
    {
        CheckState();
        currentState?.OnStateUpdate();
        WallRunClimbCooldown();
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

        // Just to force the player from the wall if they stick to it too long for my taste, i will add a froce that pushes them off the wall
        // so this is another redundancy in my eyes
        //if((currentState == wallRunning || currentState == climbWall) && wallRunTimer <= 0)
        //{
        //    ChangeState(airBourne);
        //    return;
        //}
        // JumpFromWall
        if ((leftWallrunEnabled || rightWallrunEnabled || frontWallClimbEnabled) && pc.jumpPressed && AboveGround())
        {
            ChangeState(jumpingFromWall);
        }
        // Wallrunning
        else if ((leftWallrunEnabled || rightWallrunEnabled) && pc.climbPressed && !isGrounded && AboveGround() && pc.endWallRunTimer <= 0 && !IsOnWallCooldown)// maybe i should move more things from playerController in here
        {
            ChangeState(wallRunning);
        }
        // Climbing, i think i can use this in other Interactables or better Scalable objects, as soon as i create them
        else if (frontWallClimbEnabled && pc.climbPressed && !leftWallrunEnabled && !rightWallrunEnabled && pc.endWallRunTimer <= 0 && !IsOnWallCooldown)
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

        Ray ray = new(cameraTransform.transform.position, Vector3.forward);
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
    public void ResetWallInteractionTimer()
    { 
        // Reset only when i tell you
        wallRunTimer = maxWallRunTime;
    }
    public void DecrementWallInteractionTimer(float deltaTime)
    {
        // Count the timer down, if this hits 0 i will tell the Climbwall and wallrun script to push someone off
        if( wallRunTimer > 0 )
        { wallRunTimer -= deltaTime; }
    }
#endregion
    public void ChangeState(State newState)
    {
        if(newState == currentState) return;

        // Cooldown Logic...
        // Somehow recognize if the current state was wall related, run or climb
        bool wasInWallContact = (currentState == wallRunning || currentState == climbWall);
        // recognize if the new state is wall related, run or climb
        // or rather not, since that can initate the timer to reset
        bool willNotBeInWallContact = (newState != wallRunning && newState != climbWall);
        // if so, do NOT touch the timer that should force the player from the wall at some point, to not exploit the features
        // and do reset the timer only after they left the wall really
        // ... ohoh, climb adds froce to up. increase the force that pushes the player from the wall
        // Reset cooldown if player leaves the wall somehow
        if(wasInWallContact && willNotBeInWallContact)
        {
            wallRunClimbCooldownTimer = wallRunClimbCooldown;
            IsOnWallCooldown=true;
        }
        else if(newState == jumpingFromWall)
        {
            wallRunClimbCooldownTimer = wallRunClimbCooldown;
            IsOnWallCooldown = true;
        }
        // Universal Timer Logic for wallrun AND climb
        // Reset Timer for running/or climbing the wall only if it entered from a non wall related state
        bool entersWallLegit = (newState == wallRunning || newState == climbWall) // ok sorry this is getting ridicolous
            && !(currentState == wallRunning || currentState == climbWall || currentState == jumpingFromWall);
        // Only if this bloat is the case you may reset the timer
        if (entersWallLegit )
        {
            ResetWallInteractionTimer();
        }
        currentState?.OnStateExit();
        currentState = newState;
        currentState.OnStateEnter(this, pc);
    }
    public void SetMoveInput(Vector2 moveInput)
    {
        currentInput = moveInput;
    }
}
