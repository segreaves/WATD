using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(ForceReceiver))]
public class AgentMovement : MonoBehaviour
{

    [field: SerializeField] public MovementDataSO MovementData { get; set; }

    public CharacterController Controller { get; private set; }
    private float currentVelocity;
    public Vector3 lastDirection { get; private set; }
    private Quaternion currentRotation;
    private Vector3 currentMotion;
    private ForceReceiver ForceReceiver;
    private Animator Animator;
    protected readonly int SpeedHash = Animator.StringToHash("Speed");
    protected readonly int ForwardSpeedHash = Animator.StringToHash("ForwardSpeed");
    protected readonly int RightSpeedHash = Animator.StringToHash("RightSpeed");
    public float CurrentVelocity => Controller.velocity.magnitude;
    public float CurrentForwardVelocity => Vector3.Dot(Controller.velocity, transform.forward);
    public float CurrentRightVelocity => Vector3.Dot(Controller.velocity, transform.right);
    public bool IsMoving => Controller.velocity.sqrMagnitude > 0.01f;
    private bool isWalking = false;

    
    private void Awake()
    {
        Controller = GetComponentInChildren<CharacterController>();
        ForceReceiver = GetComponent<ForceReceiver>();
        Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        lastDirection = transform.forward;
    }

    private void Update()
    {
        UpdateAnimationData(Time.deltaTime);
    }

    public void Move(Vector3 movement)
    {
        if (movement != Vector3.zero)
        {
            currentMotion = movement;
        }
        Controller.Move((currentMotion * CalculateSpeed(movement) + ForceReceiver.Movement) * Time.deltaTime);
    }

    protected void UpdateAnimationData(float deltaTime)
    {
        // Speed
        Animator.SetFloat(SpeedHash, CurrentVelocity, 0.025f, Time.deltaTime);
        // Forward speed
        Animator.SetFloat(ForwardSpeedHash, CurrentForwardVelocity, 0.025f, deltaTime);
        // Right speed
        Animator.SetFloat(RightSpeedHash, CurrentRightVelocity, 0.025f, deltaTime);
    }

    public void Move()
    {
        Move(Vector3.zero);
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
        float targetSpeed = isWalking ? MovementData.walkSpeed : MovementData.maxSpeed;
        currentVelocity = Mathf.Clamp(currentVelocity, 0f, targetSpeed);
        return currentVelocity;
    }

    public void FaceDirection(Vector3 look)
    {
        if (look == Vector3.zero) { return; }
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(look),
            Time.deltaTime * MovementData.rotationSpeed);
    }

    public void FaceDirection(Vector3 look, float rotationSpeed)
    {
        if (look == Vector3.zero) { return; }
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(look),
            Time.deltaTime * rotationSpeed);
    }

    public void Walk(bool walk)
    {
        isWalking = walk;
    }

    public void SetLastDirection(Vector3 newDirection)
    {
        lastDirection = newDirection;
    }
}
