using UnityEngine;
using UnityEngine.XR;
// I just want to keep my States clean
public class StateController : MonoBehaviour
{
    public PlayerController playerController;

    State currentState;

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
        playerController = GetComponent<PlayerController>();
        //Instantiate the States to ensure they are ready throughout the LifeCycle
        idleState = new();
        groundRunning = new();
        wallRunning = new();
        jumpingFromGround = new();
        jumpingFromWall = new();
        climbWall = new();
        airBourne = new();

        ChangeState(idleState);
    }
    public void HandleState()
    {
        currentState?.OnStateUpdate();
    }
    // Update is called once per frame
    void Update()
    {
        if (currentState != null)
        {
            currentState.OnStateUpdate();
        }
    }
    public void FixedUpdate()
    {
        currentState?.OnFixedUpdate();
    }
    public void ChangeState(State newState)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }
        currentState = newState;
        currentState.OnStateEnter(this);
    }
    public void ProcessInput()
    {

    }
}
