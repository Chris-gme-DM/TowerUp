using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
// I just want to keep my States clean
public class StateController : MonoBehaviour
{
    public PlayerController playerController;
    public Vector2 currentInput;

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
        currentState?.OnStateUpdate();
    }
    public void FixedUpdate()
    {
        currentState?.OnStateFixedUpdate();
    }
    public void ChangeState(State newState)
    {
        if(newState == currentState) return;
        currentState?.OnStateExit();
        currentState = newState;
        currentState.OnStateEnter(this, playerController);
        Debug.Log(newState);
    }
    public void SetMoveInput(Vector2 moveInput)
    {
        currentInput = moveInput;
    }
}
