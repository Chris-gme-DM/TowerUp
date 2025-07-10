using System;
using UnityEngine;

public class WallRunning : State
{
    //private Quaternion currentRotation;
    //private Quaternion targetRotation;
    public override void OnEnter()
    {
        base.OnEnter();
        // Clamp velocity to prevent infinite acceleration
        rb.maxLinearVelocity = pc.wallRunSpeed;
        // Reset Timer when starting wall run, moved to someone responsible

        //rb.freezeRotation = false; // i made a small mistake and i know it but i'm too tired to deal with this now

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

        Vector3 wallNormal = sc.rightWallrunEnabled ? sc.rightWallHit.normal : sc.leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, rb.transform.up);

        // To determin the direction the player wants to run along the wall
        if((cameraTransform.transform.forward - wallForward).magnitude > (cameraTransform.transform.forward - -wallForward).magnitude)
            wallForward = -wallForward;
        // Force forward
        rb.AddForce(wallForward * pc.wallRunForce, ForceMode.Force);

        // Adjust rotation for immersion while running the wall, readjust the rotation while airbourne
        //targetRotation = Quaternion.AngleAxis(tiltAngle, tiltAxis);
        //currentRotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Pin the player to wall
        if (!(sc.leftWallrunEnabled && moveInput.sqrMagnitude > 0) && !(sc.rightWallrunEnabled && moveInput.sqrMagnitude > 0))
            rb.AddForce(-wallNormal*10, ForceMode.Force);

    }

    public override void OnExit()
    {
        base.OnExit();
        pc.endWallRunTimer = pc.endWallRunTime;
        rb.maxLinearVelocity = 50f;
    }
}
