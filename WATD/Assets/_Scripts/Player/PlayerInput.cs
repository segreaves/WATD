using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public partial class PlayerInput : MonoBehaviour, Controls.IPlayerActions, IAgentInput
{
    private Controls controls;

    public CharacterController Controller;
    [field: SerializeField] public UnityEvent<Vector3> OnMovement { get; set; }
    [field: SerializeField] public UnityEvent<Vector3> OnFaceDirection { get; set; }
    [field: SerializeField] public UnityEvent<Vector3, float> OnRotateTowards { get; set; }
    [field: SerializeField] public UnityEvent<bool> OnWalk { get; set; }
    public event Action AttackEvent;
    public event Action<bool> AimEvent;
    public event Action DashEvent;
    public Vector3 MovementValue { get; private set; }
    public Vector3 LookValue { get; private set; }
    public Transform MainCameraTransform { get; private set; }
    public bool dashEnabled = true;
    public bool aimEnabled = true;
    public bool movementInput;
    public bool lookInput;

    private void Awake()
    {
        Controller = GetComponentInChildren<CharacterController>();
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
        if (MovementValueXY.magnitude < 0.1f)
        {
            movementInput = false;
            MovementValueXY = Vector2.zero;
        }
        else
        {
            movementInput = true;
        }
        MovementValue = CalculateDirection(MovementValueXY);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 lookValueXY = context.ReadValue<Vector2>();
        if (lookValueXY.magnitude < 0.1f)
        {
            lookInput = false;
            lookValueXY = Vector3.zero;
        }
        else
        {
            lookInput = true;
        }
        LookValue = CalculateDirection(lookValueXY);
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
        aimEnabled = context.performed;
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
        AttackEvent?.Invoke();
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
