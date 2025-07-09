using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using static UnityEngine.InputSystem.InputAction;

// I will try to rewrite PlayerController into a form of StateMachine
// I rewerote the PlayerController into a Form of InputManager who checks in which state the player should be in and what inputs he gives.
// These information will be transported to the statemachine that delivers the information. although i am sure i made redundancies
public class PlayerController : MonoBehaviour
{
    #region Unity UI
    [Header("References")]
    public StateController stateController;
    public InteractionManager interactionManager;
    public GameObject CreditsUI;
    // Variables for Script
    // Reference to Player Input
    public PlayerInput playerInput;
    // Reference to the physics of the PlayerObject
    public Rigidbody rb;
    // Movement Input reading
    public Vector2 moveInput;
    // Reference for the camera to Enable look around
    public Transform cameraTransform;
    public int credits;
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

    [Header("Jumping")]
    // To allow the setting of character Jump Force in Unity
    [Range(0f, 30f)] public float jumpForce;  // IMPULSE SETTING, be responsible
    // A Cooldwn for the JumpAction
    [Range(0f, 1f)] public float jumpCooldown;

    [Header("WallRun")]
    [Range(0f, 500f)] public float wallRunForce;
    [Range(0f, 50f)] public float wallRunSpeed;

    [Header("Climbing")]
    [Range(0f, 500f)] public float wallClimbForce;
    [Range(0f, 50f)] public float wallClimbSpeed;

    [Header("Limits")] // these are set here to keep Player related stuff as much as possible here.
                       // i should write an entire script only related to this administrative stuff
    [Range(0f, 30f)] public float wallDisengageForce; // Impulse setting, please act with caution

    // you seem redundant
    public float endWallRunTime;
    public float endWallRunTimer;

    //limitations of wallrunning and climbing, should move to StateController, maybe

    #endregion
    #region Booleans
    // To set a condition to check if the player is on a ground surface
    // Will check if Jump is pressed and based on condition if player is on ground or not give the respective StateChange
    public bool jumpPressed;
    // If climb is pressed and other climb conditions are met, the player has to hold to climb
    public bool climbPressed;
    // To set a kind of a cooldown for Jumps
    public bool canJump;
    // Set if character pressed slide
    //IF time > 0   public bool slidePressed;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get Rigidbody and PlayerInput for access
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        // Automatically find StateController and InteractionManager i took a liking to let it find things on its own
        // Safety checks in case i didn't assign any for the moment this should make things easy
        if(stateController == null)
            stateController = FindAnyObjectByType<StateController>();
        if(interactionManager == null)
            interactionManager = FindAnyObjectByType<InteractionManager>();

        // Subscribe to InputActions
        playerInput.actions["Move"].performed += onMove;
        playerInput.actions["Move"].canceled += onMove;
        playerInput.actions["Jump"].performed += onJump;
        playerInput.actions["Interact"].performed += onInteractPerformed;
        playerInput.actions["Interact"].canceled -= onInteractPerformed;
        playerInput.actions["Climb"].performed += onClimb;
        playerInput.actions["Climb"].canceled -= onClimb;
        //Define Camera transformation, in this case the camera is locked to the player
        cameraTransform = Camera.main.transform;
        // Set Jumpability
        canJump = true;
        jumpPressed = false;
        rb.useGravity = true;
        credits = 0;
    }
    private void Update()
    {
        // Count EndWallRunTimer
        if(endWallRunTimer > 0)
            endWallRunTimer -= Time.deltaTime;
    }
    // Implemented a StateMachine Architecture, which turned StateHandler() and the movementStates unnecessary.
    public void onMove(InputAction.CallbackContext ctx)
    {
        //Reads PlayerInput
        moveInput = ctx.ReadValue<Vector2>();
        //Passes every current Input on to StateMachine
        stateController.SetMoveInput(moveInput);
    }
    // JumpFunction MOVE to JumpFromWall or JumpFromGround
    public void onJump(InputAction.CallbackContext ctx) 
    {
        // Only Jump if it's not on cooldown and the Player is Grounded
        // Sets Jumping to true and together with the state if the player is on ground or not fires respective State
        if(canJump)
        { 
            canJump = false;
            jumpPressed = true;
        }
        //Set a Cooldown for the Jump
        Invoke(nameof(ResetJump), jumpCooldown);
    }
    // MOVE to JumpFromWall and JumpFrom Ground
    // A Jump Reset to not abuse the Jump mehcanic and make Wall Runs smoother
    private void ResetJump()
    {
        canJump = true;
        jumpPressed = false;
    }
    public void onInteractPerformed(InputAction.CallbackContext ctx)
    {
        if(interactionManager.currentInteractable )
        interactionManager.TriggerInteract();
    }
    public void onClimb(InputAction.CallbackContext ctx)
    {
        if(ctx.performed) climbPressed = true;
        if(ctx.canceled) climbPressed = false;
    }
}