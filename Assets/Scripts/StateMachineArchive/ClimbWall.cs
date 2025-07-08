using System;
using UnityEngine;

public class ClimbWall : State
{
    public override void OnEnter()
    {
        base.OnEnter();
        rb.useGravity = false;
        rb.maxLinearVelocity = pc.wallClimbSpeed;
        // Moved the timer reset to someone with responsibility
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        // aformentioned responsible script
        sc.DecrementWallInteractionTimer(Time.deltaTime);
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        rb.useGravity = false;
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

        Vector3 wallNormal = sc.frontWallHit.normal;
        // Climp Up
        rb.AddForce(rb.transform.up * pc.wallClimbForce, ForceMode.Force);
        // Force the player from the wall as soon as the timer runs out
        if (sc.wallRunTimer <= 0)
        {
            pc.climbPressed = false;
            rb.AddForce(wallNormal * pc.wallDisengageForce, ForceMode.Impulse);
        }

    }
    public override void OnExit()
    {
        base.OnExit();
        rb.useGravity = true;
        pc.endWallRunTimer = pc.endWallRunTime;
        pc.climbPressed = false;
        rb.maxLinearVelocity = 50f;
    }
}
