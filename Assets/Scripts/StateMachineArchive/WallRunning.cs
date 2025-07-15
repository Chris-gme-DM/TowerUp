using System;
using UnityEngine;

public class WallRunning : State
{
    public override void OnEnter()
    {
        base.OnEnter();
        // Clamp velocity to prevent infinite acceleration
        rb.maxLinearVelocity = pc.wallRunSpeed;
        // Reset Timer when starting wall run, moved to someone responsible
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        // temporarily turns off gravity
        rb.useGravity = false;
        // horizontal velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0 , rb.linearVelocity.z);

        // to get the wall's normal it gets the information from the state controller
        Vector3 wallNormal = sc.rightWallrunEnabled ? sc.rightWallHit.normal : sc.leftWallHit.normal;
        // to determine in which direction the force is supposed to be applied to
        Vector3 wallForward = Vector3.Cross(wallNormal, rb.transform.up);

        // To determin the direction the player wants to run along the wall
        if((cameraTransform.transform.forward - wallForward).magnitude > (cameraTransform.transform.forward - -wallForward).magnitude)
            wallForward = -wallForward;
        // Force forward
        rb.AddForce(wallForward * pc.wallRunForce, ForceMode.Force);

        // Pin the player to wall
        if (!(sc.leftWallrunEnabled && moveInput.sqrMagnitude > 0) && !(sc.rightWallrunEnabled && moveInput.sqrMagnitude > 0))
            rb.AddForce(-wallNormal*10, ForceMode.Force);

    }

    public override void OnExit()
    {
        base.OnExit();
        rb.maxLinearVelocity = 50f;
    }
}
