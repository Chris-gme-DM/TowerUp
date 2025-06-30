using System;
using UnityEngine;

public class JumpFromWall : State
{
    public override void OnEnter()
    {
        base.OnEnter();
        rb.maxLinearVelocity = pc.wallRunSpeed;

        rb.AddForce(0.5f* pc.jumpForce, 0.5f*pc.jumpForce, 0.5f*pc.jumpForce, ForceMode.Force);
    }
    public override void OnExit()
    {
        base.OnExit();
        rb.maxLinearVelocity = 50f;
    }

}
