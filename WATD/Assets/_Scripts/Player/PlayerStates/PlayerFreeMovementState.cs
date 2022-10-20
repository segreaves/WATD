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
        stateMachine.Animator.CrossFadeInFixedTime(LocomotionHash, 0.2f);
        stateMachine.InputReceiver.DashEvent += OnDash;
        stateMachine.InputReceiver.AttackEvent += OnAttack;
    }

    public override void Exit()
    {
        stateMachine.InputReceiver.DashEvent -= OnDash;
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
    }

    public override void Tick(float deltaTime)
    {
        // Update movement speed
        stateMachine.InputReceiver.OnWalk?.Invoke(stateMachine.InputReceiver.lookInput);
        // Update movement
        stateMachine.InputReceiver.OnMovement?.Invoke(stateMachine.InputReceiver.MovementValue);
        // Update direction
        if (stateMachine.InputReceiver.lookInput)
        {
            stateMachine.InputReceiver.OnFaceDirection?.Invoke(stateMachine.InputReceiver.LookValue);
        }
        else
        {
            Vector3 velocity = stateMachine.InputReceiver.Controller.velocity;
            velocity.y = 0f;
            if (velocity.sqrMagnitude > 0.01f)
            {
                stateMachine.InputReceiver.OnFaceDirection?.Invoke(velocity.normalized);
            }
        }
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
}
