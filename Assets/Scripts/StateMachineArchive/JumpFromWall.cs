using System;
using UnityEngine;

public class JumpFromWall : State
{
    public override void OnEnter()
    {
        base.OnEnter();
        // Determine side of the wall
        Vector3 wallNormal = sc.rightWall ? sc.rightWallHit.normal : sc.leftWallHit.normal;

        rb.AddForce(rb.transform.up * pc.jumpForce + wallNormal * pc.jumpForce, ForceMode.Force);
    }
    public override void OnExit()
    {
        base.OnExit();
        rb.maxLinearVelocity = 50f;
    }

}
