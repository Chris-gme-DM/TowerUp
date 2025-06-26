using UnityEngine;

public abstract class State
{
    public StateController sc;

    public void OnStateEnter(StateController stateController)
    {
        sc = stateController;
        OnEnter();
    }
    protected virtual void OnEnter()
    {
        //Activate its Animation
    }
    public void OnStateUpdate() 
    {
        OnUpdate(); 
    }
    protected virtual void OnUpdate()
    {
        // Callback the Update Function of any give State
    }
    public void OnFixedUpdate() { FixedUpdate(); }
    protected virtual void FixedUpdate() { }
    public void OnStateExit() { OnExit(); }
    protected virtual void OnExit() { }
}
