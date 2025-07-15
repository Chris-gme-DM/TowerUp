using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class AirBourne : State
{
    private Vector3 horizontalVelocity;
    private Vector3 moveDirection;

    public override void OnEnter()
    {
        base.OnEnter();
        rb.useGravity = true;
        //rb.freezeRotation = false;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        // Self explanatory
        moveDirection = cameraTransform.right * moveInput.x + cameraTransform.forward * moveInput.y;
        moveDirection = Vector3.ProjectOnPlane(moveDirection, Vector3.up).normalized;

    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        rb.useGravity = true;

        // Copied GroundRunning controls for a forgiving movement in air and leave more control to the player instead of demanding real physic simulation capabilities
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        // Define horizontal Velocity to exclude y-Forces from the equations
        if (horizontalVelocity.magnitude < pc.groundSpeed)
        {
            if (moveDirection.magnitude > 0.01f)
            {
                // I like physics so I use Inertia
                rb.AddForce(moveDirection * pc.accelaration*0.1f, ForceMode.Force);
            }
            // i set a max velocity although this needs to be reevaluated
            if (horizontalVelocity.magnitude > pc.groundSpeed)
            {
                Vector3 limitedHorizonatlVelocity = horizontalVelocity.normalized * pc.groundSpeed;
                rb.linearVelocity = new Vector3(limitedHorizonatlVelocity.x, 0, limitedHorizonatlVelocity.z);
            }
        }
    }
    public override void OnExit()
    {
        base.OnExit();
    }
}
