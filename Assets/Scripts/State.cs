using UnityEngine;

public abstract class State
{
    protected PlayerController pc;
    protected StateController sc;
    protected Rigidbody rb;
    protected Transform cameraTransform;
    protected Vector2 moveInput;
    public void OnStateEnter(StateController stateController, PlayerController playerController)
    {
        sc = stateController;
        pc = playerController;
        rb = pc.rb;
        cameraTransform = pc.cameraTransform;
        OnEnter();
    }
    public virtual void OnEnter()
    {
        rb.useGravity = true;

        //Activate its Animation
    }
    public void OnStateUpdate() 
    {
        moveInput = pc.moveInput;
        OnUpdate(); 
    }
    public virtual void OnUpdate()
    {
        // Callback the Update Function of any give State
    }
    public void OnStateFixedUpdate() { OnFixedUpdate(); }
    public virtual void OnFixedUpdate()
    {
        // Continious Movement needs to be handled here
    }
    public void OnStateExit() { OnExit(); }
    public virtual void OnExit()
    {
    }
}
