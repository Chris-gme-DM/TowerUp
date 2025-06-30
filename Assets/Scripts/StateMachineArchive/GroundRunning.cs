using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GroundRunning : State
{
    private Vector3 horizontalVelocity;
    private Vector3 moveDirection;
    public override void OnEnter() { base.OnEnter(); }
    public override void OnUpdate()
    {
        base.OnUpdate();
        moveDirection = cameraTransform.right * moveInput.x + cameraTransform.forward * moveInput.y;
        moveDirection = Vector3.ProjectOnPlane(moveDirection, Vector3.up).normalized;

    }
    public override void OnFixedUpdate() 
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        // Define horizontal Velocity to exclude y-Forces from the equations
        if (horizontalVelocity.magnitude < pc.groundSpeed)
        {
            if (moveDirection.magnitude > 0.01f)
            {
                rb.AddForce(moveDirection * pc.accelaration, ForceMode.Force);
            }
            if(horizontalVelocity.magnitude > pc.groundSpeed)
            {
                Vector3 limitedHorizonatlVelocity = horizontalVelocity.normalized * pc.groundSpeed;
                rb.linearVelocity = new Vector3(limitedHorizonatlVelocity.x, 0 , limitedHorizonatlVelocity.z);
            }
        }
    
    }
    public override void OnExit()
    {
        base.OnExit();
        rb.maxLinearVelocity = 50f;
    }

}
