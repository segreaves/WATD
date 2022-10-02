using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;

    [field: SerializeField]
    public UnityEvent<Vector2> OnMovement { get; set; }
    public Vector2 MovementValue { get; private set; }

    public event Action AttackEvent;

    private void Start()
    {
        controls = new Controls();
        controls.Player.SetCallbacks(this);
        controls.Player.Enable();
    }

    private void Update()
    {
        OnMovement?.Invoke(MovementValue);
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
}
