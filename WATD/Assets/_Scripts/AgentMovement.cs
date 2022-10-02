using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AgentMovement : MonoBehaviour
{
    protected Rigidbody rigidBody;

    [field: SerializeField] public MovementDataSO MovementData { get; set; }

    [SerializeField] protected float currentVelocity;
    protected Vector3 movementDirection;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void MoveAgent(Vector2 movementInput)
    {
        if (movementInput.magnitude > 0)
        {
            Vector3 movement = new Vector3(movementInput.x, 0f, movementInput.y);
            //if (Vector2.Dot(movement, movementDirection) < 0) { currentVelocity = 0; }
            movementDirection = movement.normalized;
        }
        currentVelocity = CalculateSpeed(movementInput);
    }

    private float CalculateSpeed(Vector2 movementInput)
    {
        Debug.Log(movementInput.magnitude > 0);
        if (movementInput.magnitude > 0)
        {
            currentVelocity += MovementData.acceleration * Time.deltaTime;
        }
        else
        {
            currentVelocity -= MovementData.deceleration * Time.deltaTime;
        }
        return Mathf.Clamp(currentVelocity, 0f, MovementData.maxSpeed);
    }

    private void FixedUpdate()
    {
        rigidBody.velocity = currentVelocity * movementDirection;
    }
}
