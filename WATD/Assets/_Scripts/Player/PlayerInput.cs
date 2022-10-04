using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;

    [field: SerializeField] public UnityEvent<Vector2> OnMovement { get; set; }
    [field: SerializeField] public UnityEvent<Vector3> OnFaceDirection { get; set; }
    public Vector2 MovementValue { get; private set; }
    public Vector3 LookValue { get; private set; }
    public Transform MainCameraTransform { get; private set; }

    public event Action AttackEvent;

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

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        AttackEvent?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
        if (MovementValue.magnitude < 0.1f) {
            MovementValue = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();
        LookValue = CalculateLook(lookInput);
        if (lookInput.magnitude < 0.1f) {
            LookValue = Vector3.zero;
        }
    }

    public Vector3 CalculateLook(Vector2 lookInput)
    {
        // Camera forward vector
        Vector3 forward =  MainCameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();
        // Camera right vector
        Vector3 right =  MainCameraTransform.right;
        right.y = 0f;
        right.Normalize();
        return forward * lookInput.y +
            right * lookInput.x;
    }
}
