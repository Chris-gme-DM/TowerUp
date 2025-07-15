using System;
using UnityEngine;

public class IdleState : State
{
    public override void OnEnter()
    {
        base.OnEnter();
        // Set animation
        // Reset values of ground based Movements?
        
    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        // Apply gravity and halt the player
        rb.useGravity = true;

        // Deceleration of the player
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        // Redundancies from times before the state machine
        if(horizontalVelocity.magnitude > 0.01f)
        {
            // Decelerate the player to counteract the inertia the player experiences while running, there is no walking
            rb.AddForce(horizontalVelocity * Mathf.Max(-pc.decelaration, -rb.linearVelocity.magnitude), ForceMode.Force);
            if (horizontalVelocity.magnitude > 0.1f)
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }

    }

}
