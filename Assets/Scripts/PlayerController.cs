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
    [Range(150f, 300f)] public float jumpForce;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        playerInput.actions["Move"].performed += onMove;
        playerInput.actions["Move"].canceled += onMove;
        playerInput.actions["Jump"].performed += onJump;

        cameraTransform = Camera.main.transform;
    }

    public void onMove(CallbackContext ctx)
    {
        //Reads PlayerInput
        moveInput = ctx.ReadValue<Vector2>();
    }
    public void onJump(CallbackContext ctx) 
    {
        if (!isGrounded) return;
        rb.AddForce(Vector3.up * jumpForce);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (moveInput != Vector2.zero)
        {
            var moveDirection = cameraTransform.right * moveInput.x + cameraTransform.forward * moveInput.y;
            moveDirection = Vector3.ProjectOnPlane(moveDirection, Vector3.up).normalized;
            if (!isGrounded) return;
            rb.AddForce(moveDirection * accelaration, ForceMode.Force);
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.maxLinearVelocity = maxSpeed;
            }
        //    rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);
        }
        else
        {
            rb.AddForce(rb.linearVelocity * Mathf.Max(-decelaration, -rb.linearVelocity.magnitude), ForceMode.Force);
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
