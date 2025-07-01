using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class AirBourne : State
{
    private Vector3 horizontalVelocity;
    private Vector3 moveDirection;

    public float rotationSpeed = 3.0f;
    public float tiltAngle = 0f;

    private Quaternion currentRotation;
    private Quaternion targetRotation;

    public override void OnEnter()
    {
        base.OnEnter();
        rb.useGravity = true;
        //rb.freezeRotation = false;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        moveDirection = cameraTransform.right * moveInput.x + cameraTransform.forward * moveInput.y;
        moveDirection = Vector3.ProjectOnPlane(moveDirection, Vector3.up).normalized;

    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        rb.useGravity = true;

        // Copied GroundRunning controls for a forgiving movement in air and leave more control to the player instead of demanding real physic simulation capabilities
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        // Define horizontal Velocity to exclude y-Forces from the equations
        if (horizontalVelocity.magnitude < pc.groundSpeed)
        {
            if (moveDirection.magnitude > 0.01f)
            {
                rb.AddForce(moveDirection * pc.accelaration*0.15f, ForceMode.Force);
            }
            if (horizontalVelocity.magnitude > pc.groundSpeed)
            {
                Vector3 limitedHorizonatlVelocity = horizontalVelocity.normalized * pc.groundSpeed;
                rb.linearVelocity = new Vector3(limitedHorizonatlVelocity.x, 0, limitedHorizonatlVelocity.z);
            }
        }
        // Adjust rotation back to normal
        //Vector3 tiltAxis = Vector3.Cross(rb.transform.right, rb.transform.up).normalized;
        //targetRotation = Quaternion.AngleAxis(tiltAngle, tiltAxis);
        //currentRotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);

    }
    public override void OnExit()
    {
        base.OnExit();
        //rb.freezeRotation = true;
    }
}
