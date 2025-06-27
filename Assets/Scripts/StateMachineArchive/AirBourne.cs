using System;
using UnityEngine;

public class AirBourne : State
{
    public override void OnEnter()
    {
        base.OnEnter();
    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        rb.useGravity = true;
    }
    public override void OnExit() { base.OnExit(); }
}
