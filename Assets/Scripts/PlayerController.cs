using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
public class PlayerController : MonoBehaviour
{
    // Values to set for developer in Unity
    // Accelaration Value
    [Range(0f, 50f)] public float accelaration;
    // Decelaration Value
    [Range(0f, 50f)] public float decelaration;
    // To allow the setting of character movement Speed in Unity
    [Range(0f, 50f)] public float maxSpeed;
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

    // To set a condition to check if the player is on a ground surface
    private bool isGrounded;
    // To set a kind of a cooldown for Jumps
    private bool canJump;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        playerInput.actions["Move"].performed += onMove;
        playerInput.actions["Move"].canceled += onMove;
        playerInput.actions["Jump"].performed += onJump;

        cameraTransform = Camera.main.transform;
        canJump = true;
    }

    public void onMove(CallbackContext ctx)
    {
        //Reads PlayerInput
        moveInput = ctx.ReadValue<Vector2>();
    }
    public void onJump(CallbackContext ctx) 
    {
        if (!isGrounded || !canJump) return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Force);

        canJump = false;
        Invoke(nameof(ResetJump), jumpCooldown);
    }
    // A Jump Reset to not abuse the Jump mehcanic and make Wall Runs smoother
    private void ResetJump()
    {
        canJump = true;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (moveInput != Vector2.zero)
        {
            var moveDirection = cameraTransform.right * moveInput.x + cameraTransform.forward * moveInput.y;
            moveDirection = Vector3.ProjectOnPlane(moveDirection, Vector3.up).normalized;
            if (!isGrounded) return;
            if (horizontalVelocity.magnitude < maxSpeed)
            {
                rb.AddForce(moveDirection * accelaration, ForceMode.Force);
            }
        }
        // Deceleration of the player
        else if(isGrounded)
        {
            rb.AddForce(horizontalVelocity * Mathf.Max(-decelaration, -rb.linearVelocity.magnitude), ForceMode.Force);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
