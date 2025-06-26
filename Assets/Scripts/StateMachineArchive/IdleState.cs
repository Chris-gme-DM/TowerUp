using UnityEngine;

public class IdleState : State
{
    protected override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Entered Idle State");
        // Set animation
        // Reset values of ground based Movements?
        
    }
}
