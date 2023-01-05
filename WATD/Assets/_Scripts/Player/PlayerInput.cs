using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public partial class PlayerInput : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;

    public CharacterController Controller;
    [field: SerializeField] private AnimationCurve InputMovementCurve;
    [field: SerializeField] public UnityEvent<Vector3> OnMovement { get; set; }
    [field: SerializeField] public UnityEvent<Vector3> OnLookAt { get; set; }
    [field: SerializeField] public UnityEvent<Vector3> OnFaceDirection { get; set; }
    [field: SerializeField] public UnityEvent<Vector3, float> OnRotateTowards { get; set; }
    public event Action AttackEvent;
    public event Action ShootEvent;
    public event Action<bool> AimEvent;
    public bool IsAimingPressed;
    public event Action DashEvent;
    public Vector3 MovementValue { get; private set; }
    public Vector3 LookValue { get; private set; }
    public Transform MainCameraTransform { get; private set; }
    private Animator Animator;
    public bool dashEnabled = true;
    public bool movementInput;
    public bool lookInput;
    public bool IsInteracting;

    private void Awake()
    {
        Controller = GetComponentInChildren<CharacterController>();
        Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Set main camera transform
        MainCameraTransform = Camera.main.transform;
        // Initialize controls
        controls = new Controls();
        controls.Player.SetCallbacks(this);
        controls.Player.Enable();
    }

    private void OnDestroy()
    {
        controls.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 MovementValueXY = context.ReadValue<Vector2>();
        float inputMagnitude = InputMovementCurve.Evaluate(MovementValueXY.magnitude);
        if (inputMagnitude <= 0.2f)
        {
            movementInput = false;
            MovementValue = Vector3.zero;
        }
        else
        {
            movementInput = true;
            MovementValue = CalculateDirection(MovementValueXY).normalized * inputMagnitude;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 lookValueXY = context.ReadValue<Vector2>();
        if (lookValueXY.magnitude < 0.5f)
        {
            lookInput = false;
            LookValue = Vector3.zero;
        }
        else
        {
            lookInput = true;
            LookValue = CalculateDirection(lookValueXY);
        }
    }

    public Vector3 CalculateDirection(Vector2 xyValue)
    {
        // Camera forward vector
        Vector3 forward = MainCameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();
        // Camera right vector
        Vector3 right = MainCameraTransform.right;
        right.y = 0f;
        right.Normalize();
        return forward * xyValue.y +
            right * xyValue.x;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        Debug.Log(context.performed);
        IsAimingPressed = context.performed;
        AimEvent?.Invoke(context.performed);
    }

    public IEnumerator EDashCooldown(float duration)
    {
        dashEnabled = false;
        yield return new WaitForSeconds(duration);
        dashEnabled = true;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        if (IsAimingPressed == true)
        {
            ShootEvent?.Invoke();
        }
        else
        {
            AttackEvent?.Invoke();
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        if (!dashEnabled) { return; }
        if (MovementValue.magnitude < 0.1f) { return; }
        DashEvent?.Invoke();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // Movement direction
            if (MovementValue != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position, MovementValue * 2f);
            }
            // Look direction
            if (LookValue != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, LookValue * 2f);
            }
        }
    }
}
