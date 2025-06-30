using System;
using UnityEngine;

public class JumpFromWall : State
{
    public override void OnEnter()
    {
        base.OnEnter();
        // Determine side of the wall
        Vector3 wallNormal = pc.rightWall ? pc.rightWallHit.normal : pc.leftWallHit.normal;

        rb.AddForce(rb.transform.up * pc.jumpForce * 0.7f + wallNormal * pc.jumpForce * 0.7f, ForceMode.Force);
    }
    public override void OnExit()
    {
        base.OnExit();
        rb.maxLinearVelocity = 50f;
    }

}
