using System;
using UnityEngine;

public class JumpFromWall : State
{
    public override void OnEnter()
    {
        base.OnEnter();
        // Determine side of the wall
        Vector3 wallNormal = sc.rightWallrunEnabled ? sc.rightWallHit.normal : sc.leftWallHit.normal;
        if (wallNormal == null || (!sc.rightWallrunEnabled && !sc.leftWallrunEnabled && sc.frontWallClimbEnabled))
        {
            wallNormal = sc.frontWallHit.normal;
        }
        // Push off the wall, should/maybe tweak some numbers
        rb.AddForce(rb.transform.up * pc.jumpForce + wallNormal * pc.jumpForce, ForceMode.Impulse);
    }
    public override void OnExit()
    {
        base.OnExit();
        rb.maxLinearVelocity = 50f;
    }

}
