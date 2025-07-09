using System;
using UnityEngine;

public class JumpFromGround : State
{
    public override void OnEnter()
    {
        base.OnEnter();
        
        rb.AddForce(Vector3.up * pc.jumpForce, ForceMode.Impulse);

    }
}
