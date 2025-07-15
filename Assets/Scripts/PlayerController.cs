using System.Collections;
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
    // Variables for Script
    // Reference to Player Input
    public PlayerInput playerInput;
    // Reference to the physics of the PlayerObject
    public Rigidbody rb;
    // Movement Input reading
    public Vector2 moveInput;
    // Reference for the camera to Enable look around
    public Transform cameraTransform;
    // Values to set for developer in Unity
    [Header("Movement")]
    // Accelaration Value
    [Range(0f, 50f)] public float accelaration; // 50
    // Decelaration Value
    [Range(0f, 50f)] public float decelaration; // 45
    // To allow the setting of character movement Speed in Unity
    // Max Speed variable, conditional upon settings of different states
    [Range(0f, 50f)] public float maxSpeed; // 40
    // Speed when character is running on ground
    [Range(0f, 50f)] public float groundSpeed; // 10
    // Speed when the character is on a slope
 //ADD IF time >= 0   [Range(0f, 50f)] public float maxSlopeSpeed;
    // Speed when the character is Wallrunning

    [Header("Jumping")]
    // To allow the setting of character Jump Force in Unity
    [Range(0f, 30f)] public float jumpForce;  // IMPULSE SETTING, be responsible , 7f
    // A Cooldwn for the JumpAction
    [Range(0f, 1f)] public float jumpCooldown; // 0.8f

    [Header("WallRun")]
    [Range(0f, 500f)] public float wallRunForce; // 20
    [Range(0f, 50f)] public float wallRunSpeed; // 12

    [Header("Climbing")]
    [Range(0f, 500f)] public float wallClimbForce; // 40
    [Range(0f, 50f)] public float wallClimbSpeed; // 4

    [Header("Limits")] // these are set here to keep Player related stuff as much as possible here.
                       // i should write an entire script only related to this administrative stuff
    [Range(0f, 30f)] public float wallDisengageForce; // Impulse setting, please act with caution , 20 i think this one has become obsolete

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

    void Awake()
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
        playerInput.actions["Climb"].performed += onClimb;
        playerInput.actions["Climb"].canceled += onClimb;
        //Define Camera transformation, in this case the camera is locked to the player
        cameraTransform = Camera.main.transform;
        // Set Jumpability
        canJump = true;
        jumpPressed = false;
        rb.useGravity = true;
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
            jumpPressed = true;
            // Only set jumpPressed true momentarily, since Gametest found that exploits were possible, but conflicts with the Cooldown arose
            StartCoroutine(SetJumpPressedMomentarily());
            canJump = false;
        }
        //Set a Cooldown for the Jump
        Invoke(nameof(ResetJump), jumpCooldown);
    }
    private IEnumerator SetJumpPressedMomentarily()
    {
            yield return new WaitForSeconds(0.1f);
            jumpPressed = false;

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
        if (ctx.performed)
            climbPressed = true;
        else if (ctx.canceled)
            climbPressed = false;
        
    }
}