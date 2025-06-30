using System;
using UnityEngine;

public class WallRunning : State
{
    public float WallRunTimer;

    public override void OnEnter()
    {
        base.OnEnter();
        rb.maxLinearVelocity = pc.wallRunSpeed;
        WallRunTimer = 0;
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

        Vector3 wallNormal = pc.rightWall ? pc.rightWallHit.normal : pc.leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, rb.transform.up);

        // To determin the direction the player wants to run along the wall
        if((cameraTransform.transform.forward - wallForward).magnitude > (cameraTransform.transform.forward - -wallForward).magnitude)
            wallForward = -wallForward;
        // Force forward
        rb.AddForce(wallForward * pc.wallRunForce, ForceMode.Force);

        // Pin the player to wall
        if(!(pc.leftWall && moveInput.sqrMagnitude > 0) && !(pc.rightWall && moveInput.sqrMagnitude > 0))
            rb.AddForce(-wallNormal*10, ForceMode.Force);
    }
    public override void OnExit()
    {
        base.OnExit();
        WallRunTimer = 0;
        rb.maxLinearVelocity = 50f;
    }
}
