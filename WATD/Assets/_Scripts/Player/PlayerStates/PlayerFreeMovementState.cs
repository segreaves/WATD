using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeMovementState : State
{
    public PlayerFreeMovementState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    protected readonly int LocomotionHash = Animator.StringToHash("Locomotion");

    public override void Enter()
    {
        if (stateMachine.isMovementState == false)
        {
            stateMachine.Animator.CrossFadeInFixedTime(LocomotionHash, 0.2f);
        }
        stateMachine.isMovementState = IsMovementState();
        stateMachine.InputReceiver.AttackEvent += OnAttack;
        stateMachine.InputReceiver.DashEvent += OnDash;
        stateMachine.InputReceiver.AimEvent += OnAim;
        // If aim is pressed then switch immediately to aiming state
        if (stateMachine.InputReceiver.aimEnabled)
        {
            stateMachine.SwitchState(new PlayerAimingState(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
        stateMachine.InputReceiver.DashEvent -= OnDash;
        stateMachine.InputReceiver.AimEvent -= OnAim;
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.InputReceiver.OnWalk?.Invoke(stateMachine.InputReceiver.lookInput);
        stateMachine.InputReceiver.OnMovement?.Invoke(stateMachine.InputReceiver.MovementValue);
        UpdateFaceDirection();
    }

    private void OnDash()
    {
        if (stateMachine.InputReceiver.MovementValue.sqrMagnitude > 0)
        {
            stateMachine.SwitchState(new PlayerDashState(stateMachine));
        }
    }

    private void OnAttack()
    {
        stateMachine.SwitchState(new PlayerMeleeEntryState(stateMachine));
    }

    private void OnAim(bool enabled)
    {
        if (enabled == true)
        {
            stateMachine.SwitchState(new PlayerAimingState(stateMachine));
        }
    }

    protected override bool IsMovementState()
    {
        return true;
    }
}
