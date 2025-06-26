using Unity.VisualScripting;
using UnityEngine;

public class Wallrunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    [Range(0f, 400f)] public float wallRunForce;
    [Range(0f, 5f)] public float maxWallRunTime;
    public float wallRunTimer;

    // Inputs
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;

    // Booleans
    private bool leftWall;
    private bool rightWall;

    [Header("References")]
    public Transform orientation;
    private PlayerController pc;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerController>();
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
    private void Update()
    {
        CheckForWall();
        StateMachine();
    }
    private void FixedUpdate()
    {
        RunningWall();
    }
    private void CheckForWall()
    {
        leftWall = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
        rightWall = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        // Get Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Is it Running the wall
        if((leftWall || rightWall) && verticalInput > 0 && AboveGround())
        {
            // Start wallrunning
            if(!pc.wallRunning) StartWallRun();

        }
        else if(pc.wallRunning) EndWallRun();

    }

    private void StartWallRun()
    {
        pc.wallRunning = true;
    }

    private void RunningWall()
    {
        rb.useGravity = false;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        Vector3 wallNormal = rightWall ? rightWallHit.normal : leftWallHit.normal ;

        Vector3 wallForward = Vector3.Cross(wallNormal,transform.up) ;

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
    }

    private void EndWallRun()
    {
        pc.wallRunning = false;
    }
}
