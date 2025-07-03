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
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    public RaycastHit leftWallHit;
    public RaycastHit rightWallHit;
    // PlayerHeight to Check Raycast Hit on Ground
    public float playerHeight;
    // to check which side the wall is on
    public bool leftWall;
    public bool rightWall;
    public float wallCheckDistance;
    public float minJumpHeight;
    public bool isGrounded;

    private State currentState;

    public IdleState idleState;
    public GroundRunning groundRunning;
    public WallRunning wallRunning;
    public JumpFromGround jumpingFromGround;
    public JumpFromWall jumpingFromWall;
    public ClimbWall climbWall;
    public AirBourne airBourne;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

        ChangeState(idleState);
    }
    // Update is called once per frame
    void Update()
    {
        CheckState();
        currentState?.OnStateUpdate();
    }
    public void FixedUpdate()
    {
        currentState?.OnStateFixedUpdate();
    }
    private void CheckState()
    {
        GroundCheck();
        AboveGround();
        WallCheck();

        if ((leftWall || rightWall) && pc.jumpPressed && AboveGround())
        {
            ChangeState(jumpingFromWall);
        }
        else if ((leftWall || rightWall) && !isGrounded && AboveGround() && pc.endWallRunTimer <= 0)
        {
            ChangeState(wallRunning);
            Debug.Log("Schould be running the wall");
        }
        else if (pc.jumpPressed && isGrounded)
        {
            ChangeState(jumpingFromGround);
        }
        else if (isGrounded && currentInput != Vector2.zero)
        {
            ChangeState(groundRunning);
        }
        else if (isGrounded && currentInput == Vector2.zero)
        {
            ChangeState(idleState);
        }
        else
        {
            ChangeState(airBourne);
        }

    }
    private bool GroundCheck()
    {
        Ray ray = new(rb.transform.position, Vector3.down);
        return isGrounded = Physics.Raycast(rb.transform.position, Vector3.down, playerHeight * 0.5f, whatIsGround);
    }
    private void WallCheck()
    {
        Ray ray = new(cameraTransform.transform.position, Vector3.forward);
        leftWall = Physics.Raycast(cameraTransform.transform.position, -cameraTransform.transform.right, out leftWallHit, wallCheckDistance, whatIsWall);
        rightWall = Physics.Raycast(cameraTransform.transform.position, cameraTransform.transform.right, out rightWallHit, wallCheckDistance, whatIsWall);
        Debug.Log(leftWall);
        Debug.Log(rightWall);
    }
    private bool AboveGround()
    {
        return !Physics.Raycast(rb.transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    public void ChangeState(State newState)
    {
        if(newState == currentState) return;
        currentState?.OnStateExit();
        currentState = newState;
        currentState.OnStateEnter(this, pc);
        Debug.Log(newState);
    }
    public void SetMoveInput(Vector2 moveInput)
    {
        currentInput = moveInput;
    }
}
