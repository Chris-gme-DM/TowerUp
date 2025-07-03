using System;
using UnityEngine;

public class WallRunning : State
{
    //public float rotationSpeed = 3f;
    //public float tiltAngle = 45f;

    //private Quaternion currentRotation;
    //private Quaternion targetRotation;
    public override void OnEnter()
    {
        base.OnEnter();
        // Clamp velocity to prevent infinite acceleration
        rb.maxLinearVelocity = pc.wallRunSpeed;
        // Reset Timer when starting wall run
        pc.wallRunTimer = pc.maxWallRunTime;
        //rb.freezeRotation = false;

    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if(pc.wallRunTimer > 0)
            pc.wallRunTimer -= Time.deltaTime;

    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        // temporarily turns off gravity
        rb.useGravity = false;
        // horizontal velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0 , rb.linearVelocity.z);

        Vector3 wallNormal = sc.rightWall ? sc.rightWallHit.normal : sc.leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, rb.transform.up);

        // To determin the direction the player wants to run along the wall
        if((cameraTransform.transform.forward - wallForward).magnitude > (cameraTransform.transform.forward - -wallForward).magnitude)
            wallForward = -wallForward;
        // Force forward
        rb.AddForce(wallForward * pc.wallRunForce, ForceMode.Force);

        // Adjust rotation for immersion while running the wall, readjust the rotation while airbourne
        //Vector3 tiltAxis = Vector3.Cross(rb.transform.forward, wallNormal).normalized;
        //targetRotation = Quaternion.AngleAxis(tiltAngle, tiltAxis);
        //currentRotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Pin the player to wall
        if (!(sc.leftWall && moveInput.sqrMagnitude > 0) && !(sc.rightWall && moveInput.sqrMagnitude > 0))
            rb.AddForce(-wallNormal*10, ForceMode.Force);
        // Force the player from the wall as soon as the timer runs out
        if(pc.wallRunTimer <= 0) 
            rb.AddForce(wallNormal*10, ForceMode.Force);

    }

    public override void OnExit()
    {
        base.OnExit();
        pc.endWallRunTimer = pc.endWallRunTime;
        rb.maxLinearVelocity = 50f;
    }
}
