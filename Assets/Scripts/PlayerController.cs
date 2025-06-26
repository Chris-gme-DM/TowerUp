using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using static UnityEngine.InputSystem.InputAction;

// I will try to rewrite PlayerController into a form of StateMachine
public class PlayerController : MonoBehaviour
{
#region Unity UI
    [Header("References")]
    public StateController stateController;
    // Values to set for developer in Unity
    [Header("Movement")]
    // Accelaration Value
    [Range(0f, 50f)] public float accelaration;
    // Decelaration Value
    [Range(0f, 50f)] public float decelaration;
    // To allow the setting of character movement Speed in Unity
    // Max Speed variable, conditional upon settings of different states
    [Range(0f, 50f)] public float maxSpeed;
    // Speed when character is running on ground
    [Range(0f, 50f)] public float groundSpeed;
    // Speed when the character is on a slope
 //ADD IF time >= 0   [Range(0f, 50f)] public float maxSlopeSpeed;
    // Speed when the character is Wallrunning
    [Range(0f, 50f)] public float wallRunSpeed;

    [Header("Jumping")]
    // To allow the setting of character Jump Force in Unity
    [Range(150f, 1000f)] public float jumpForce;
    // A Cooldwn for the JumpAction
    [Range(0f, 1f)] public float jumpCooldown;


    // Variables for Script
    // Reference to Player Input
    private PlayerInput playerInput;
    // Reference to the physics of the PlayerObject
    private Rigidbody rb;
    // Movement Input reading
    private Vector2 moveInput;
    // Reference for the camera to Enable look around
    private Transform cameraTransform;

    [Header("CollisionChecks")]
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    #endregion
#region Booleans
    // To set a condition to check if the player is on a ground surface
    public bool isGrounded;
    // Will check if Jump is pressed and based on condition if player is on ground or not give the respective StateChange
    public bool jumpPressed;
    // To set a kind of a cooldown for Jumps
    public bool canJump;
    // Set if the character is currently running a wall
    public bool wallRunning;
    // Set if character pressed slide
 //IF time > 0   public bool slidePressed;
    // PlayerHeight to Check Raycast Hit on Ground
    private float playerHeight;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get Rigidbody and PlayerInput for access
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        stateController = GetComponent<StateController>();
        // Get Player Height
        playerHeight = GetComponent<CapsuleCollider>().height;
        // Subscribe to InputActions
        playerInput.actions["Move"].performed += onMove;
        playerInput.actions["Move"].canceled += onMove;
        playerInput.actions["Jump"].performed += onJump;

        //Define Camera transformation, in this case the camera is locked to the player
        cameraTransform = Camera.main.transform;
        // Set Jumpability
        canJump = true;
    }
    private void Update()
    {
        // Checks conditions and notifies the StateController about any changes
        CheckState();

        stateController.HandleState();
    }
    // Implemented a StateMachine Architecture, which turned StateHandler() and the movementStates unnecessary.
    // PlayerController should just constantly Check the State the player is in and their Input and leave State Handling to StateController
    #region StateMachine in PlayerController, which is now handled by a StateMachine Script called StateController
    //public MovementState movementState;

    //public enum MovementState
    //{
    //    idle,
    //    groundRunning,
    //    wallRunning,
    //    jumpingFromGround,
    //    jumpingFromWall,
    //    climbing,
    //    sliding,
    //    air
    //}
    //private void StateHandler()
    //{

    //    switch (movementState) 
    //    {
    //        case MovementState.idle:
    //            IdleBehaviour();
    //            break;
    //        case MovementState.groundRunning:
    //            MovePlayerOnGround();
    //            //Check if state has changed
    //            //movementState = CheckNextState();
    //            break;
    //        case MovementState.wallRunning:
    //            //DoStuffWhileRunningOnWalls();
    //            //Check if state has changed
    //            //movementState = CheckNextState();

    //            break;
    //        case MovementState.air:
    //            //DoStuffWhileInAir();
    //            //Check if state has changed
    //            //movementState = CheckNextState();

    //            break;


    //    }

    //    ////Mode grounded
    //    //if(isGrounded && moveInput != Vector2.zero)
    //    //{
    //    //    movementState = MovementState.groundRunning;
    //    //    maxSpeed = groundSpeed;

    //    //}
    //    ////Mode wallRunning
    //    //else if(wallRunning)
    //    //{
    //    //    movementState = MovementState.wallRunning;
    //    //    maxSpeed = wallRunSpeed;
    //    //}
    //    ////Mode Air
    //    //else
    //    //{
    //    //    movementState = MovementState.air;
    //    //}
    //}
    #endregion
    // Notifies the StateMachine upon any StateCheck
    public void NotifyStateChange(State newState)
    {
        stateController.ChangeState(newState);
    }
#region Checks
    // Check things
    // Constantly Checks the conditions the playr is in to notify StateChanges to StateController
    private void CheckState()
    {
        GroundCheck();
        wallRunning = WallCheck();

        if (isGrounded && moveInput == Vector2.zero)
        {
            NotifyStateChange(stateController.idleState);
        }
        else if (isGrounded && moveInput != Vector2.zero)
        {
            NotifyStateChange(stateController.groundRunning);
        }
        else if (wallRunning)
        {
            NotifyStateChange(stateController.wallRunning);
        }
        else if (jumpPressed && isGrounded)
        {
            NotifyStateChange(stateController.jumpingFromGround);
        }
        else if (jumpPressed && wallRunning)
        {
            NotifyStateChange(stateController.jumpingFromWall);
        }
        else
        {
            NotifyStateChange(stateController.airBourne);
        }
    }
    private bool GroundCheck()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight*0.5f), Color.red , 0.1f);
        return isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f, whatIsGround);
    }
    private bool WallCheck()
    {
        Ray ray = new Ray(transform.position, Vector3.forward);
        Debug.DrawRay(transform.position, transform.forward * 1f, Color.red, 0.1f);
        return Physics.Raycast(ray, 1f, whatIsWall);
    }
    public void onMove(CallbackContext ctx)
    {
        //Reads PlayerInput
        moveInput = ctx.ReadValue<Vector2>();
    }
    // JumpFunction MOVE to JumpFromWall or JumpFromGround
    public void onJump(CallbackContext ctx) 
    {
        // Only Jump if it's not on cooldown and the Player is Grounded
        // Using Force to Jump, Impulse would overwrite horizontal Velocity
    //    rb.AddForce(Vector3.up * jumpForce, ForceMode.Force);
        // Sets Jumping to true and together with the state if the player is on ground or not fires respective State
        if(canJump) { jumpPressed = true; }
        //Set a Cooldown for the Jump
        Invoke(nameof(ResetJump), jumpCooldown);
    }
    // MOVE to JumpFromWall and JumpFrom Ground
    // A Jump Reset to not abuse the Jump mehcanic and make Wall Runs smoother
    private void ResetJump()
    {
        canJump = true;
    }
    #endregion
    // MOVE MovePlayerOnGround to GroundRunning Script and adjust it
    private void MovePlayerOnGround()
    {
        // Define horizontal Velocity to exclude y-Forces from the equations
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        //if (moveInput != Vector2.zero)
        //{
        //    var moveDirection = cameraTransform.right * moveInput.x + cameraTransform.forward * moveInput.y;
        //    moveDirection = Vector3.ProjectOnPlane(moveDirection, Vector3.up).normalized;
        //    if (!isGrounded) return;
        //    if (horizontalVelocity.magnitude < maxSpeed)
        //    {
        //        rb.AddForce(moveDirection * accelaration, ForceMode.Force);
        //    }
        //}
        //// Deceleration of the player
        //else if(isGrounded)
        //{
        //    rb.AddForce(horizontalVelocity * Mathf.Max(-decelaration, -rb.linearVelocity.magnitude), ForceMode.Force);
        //}

    }



// Since GroundCheck() should make CollisionChecks obsolete
    //private void OnCollisionStay(Collision collision)
    //{
    //    if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
    //    isGrounded = true;
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
    //    isGrounded = false;
    //}
}