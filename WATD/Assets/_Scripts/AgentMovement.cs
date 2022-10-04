using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AgentMovement : MonoBehaviour
{
    protected Rigidbody rigidBody;
    [SerializeField] bool canMove = true;

    [field: SerializeField] public MovementDataSO MovementData { get; set; }

    protected float currentVelocity;
    protected Quaternion currentRotation;
    protected Vector3 movementDirection;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        currentRotation = rigidBody.rotation;
    }

    public void MoveAgent(Vector2 movementInput)
    {
        if (movementInput.magnitude > 0)
        {
            Vector3 movement = new Vector3(movementInput.x, 0f, movementInput.y);
            movementDirection = movement.normalized;
        }
        currentVelocity = CalculateSpeed(movementInput);
    }

    private float CalculateSpeed(Vector2 movementInput)
    {
        if (movementInput.magnitude > 0)
        {
            currentVelocity += MovementData.acceleration * Time.deltaTime;
        }
        else
        {
            currentVelocity -= MovementData.deceleration * Time.deltaTime;
        }
        return Mathf.Clamp(currentVelocity, rigidBody.velocity.y, MovementData.maxSpeed);
    }

    public void FaceDirection(Vector3 lookInput)
    {
        if (lookInput == Vector3.zero) { return; }
        currentRotation = Quaternion.Lerp(
            currentRotation,
            Quaternion.LookRotation(lookInput),
            Time.deltaTime * MovementData.rotationSpeed);
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            rigidBody.velocity = currentVelocity * movementDirection;
            rigidBody.rotation = currentRotation;
        }
    }
}
