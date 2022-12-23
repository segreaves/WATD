using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimingState : PlayerMovementStateBase
{
    public PlayerAimingState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    private float lookAngle;

    public override void Enter()
    {
        base.Enter();
        // Right arm layer
        stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("ArmR"), 1f);
        stateMachine.InputHandler.AttackEvent += OnAttack;
        stateMachine.InputHandler.DashEvent += OnDash;
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.InputHandler.AttackEvent -= OnAttack;
        stateMachine.InputHandler.DashEvent -= OnDash;
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (stateMachine.InputHandler.lookInput)
        {
            stateMachine.InputHandler.OnMovement?.Invoke(Vector3.ClampMagnitude(stateMachine.InputHandler.MovementValue, 0.3f));
        }
        else
        {
            if (stateMachine.AgentMovement.isWalking)
            {
                stateMachine.InputHandler.OnMovement?.Invoke(stateMachine.InputHandler.MovementValue.normalized);
            }
            else
            {
                stateMachine.InputHandler.OnMovement?.Invoke(stateMachine.InputHandler.MovementValue);
            }
        }
    }

    private void OnDash()
    {
        if (stateMachine.InputHandler.IsInteracting == true) { return; }
        if (stateMachine.InputHandler.MovementValue.sqrMagnitude <= 0) { return; }
        stateMachine.SwitchState(new PlayerDashState(stateMachine));
    }

    private void OnAttack()
    {
        if (stateMachine.InputHandler.IsInteracting == true) { return; }
        stateMachine.SwitchState(new PlayerMeleeEntryState(stateMachine));
    }
}
