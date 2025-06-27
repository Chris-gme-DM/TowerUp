using System;
using UnityEngine;

public class IdleState : State
{
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Entered Idle State");
        // Set animation
        // Reset values of ground based Movements?
        
    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        // Apply gravity and halt the player
        rb.useGravity = true;

        // Deceleration of the player
        Vector3 transform = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z);

        {
            rb.AddForce(transform * Mathf.Max(-pc.decelaration, -rb.linearVelocity.magnitude), ForceMode.Force);
        }

    }

}
