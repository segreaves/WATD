using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(ForceReceiver))]
public class AgentMovement : MonoBehaviour
{

    [field: SerializeField] public MovementDataSO MovementData { get; set; }

    CharacterController controller;
    private float currentVelocity;
    private Quaternion currentRotation;
    private Vector3 currentMotion;
    private Vector3 movementDirection = Vector3.zero;
    private ForceReceiver forceReceiver;

    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        forceReceiver = GetComponent<ForceReceiver>();
    }

    public void Move(Vector3 movement)
    {
        if (movement != Vector3.zero)
        {
            movementDirection = movement;
        }
        controller.Move(movementDirection * CalculateSpeed(movement) + forceReceiver.Movement * Time.deltaTime);
    }

    private float CalculateSpeed(Vector3 movementInput)
    {
        if (movementInput.magnitude > 0)
        {
            currentVelocity += MovementData.acceleration * Time.deltaTime;
        }
        else
        {
            currentVelocity -= MovementData.deceleration * Time.deltaTime;
        }
        currentVelocity = Mathf.Clamp(currentVelocity, 0f, MovementData.maxSpeed);
        return currentVelocity;
    }

    public void Look(Vector3 look)
    {
        if (look == Vector3.zero) { return; }
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(look),
            Time.deltaTime * MovementData.rotationSpeed);
    }
}
