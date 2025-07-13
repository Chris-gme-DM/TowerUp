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
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        rb.useGravity = false;
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

        Vector3 wallNormal = sc.frontWallHit.normal;
        // Climp Up
        rb.AddForce(rb.transform.up * pc.wallClimbForce, ForceMode.Force);

    }
    public override void OnExit()
    {
        base.OnExit();
        rb.useGravity = true;
        pc.climbPressed = false;
        rb.maxLinearVelocity = 50f;
    }
}
