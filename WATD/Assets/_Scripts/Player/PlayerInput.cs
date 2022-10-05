using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour, Controls.IPlayerActions, IAgentInput
{
    private Controls controls;

    [field: SerializeField] public UnityEvent<Vector3> OnMovement { get; set; }
    [field: SerializeField] public UnityEvent<Vector3> OnFaceDirection { get; set; }
    [field: SerializeField] public UnityEvent OnAttack { get; set; }
    public Vector3 MovementValue { get; private set; }
    public Vector3 LookValue { get; private set; }
    public Transform MainCameraTransform { get; private set; }


    private void Start()
    {
        // Set main camera transform
        MainCameraTransform = Camera.main.transform;
        // Initialize controls
        controls = new Controls();
        controls.Player.SetCallbacks(this);
        controls.Player.Enable();
    }

    private void Update()
    {
        OnMovement?.Invoke(MovementValue);
        OnFaceDirection?.Invoke(LookValue);
    }

    private void OnDestroy()
    {
        controls.Player.Disable();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        OnAttack?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 MovementValueXY = context.ReadValue<Vector2>();
        if (MovementValueXY.magnitude < 0.1f)
        {
            MovementValueXY = Vector2.zero;
        }
        MovementValue = CalculateDirection(MovementValueXY);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 lookValueXY = context.ReadValue<Vector2>();
        if (lookValueXY.magnitude < 0.1f)
        {
            lookValueXY = Vector3.zero;
        }
        LookValue = CalculateDirection(lookValueXY);
    }

    protected Vector3 CalculateDirection(Vector2 xyValue)
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
}
