using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeMovementState : PlayerMovementStateBase
{
    public PlayerFreeMovementState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        // Right arm layer
        stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("ArmR"), 1f);
        stateMachine.Animator.SetBool(LArmOutHash, false);
        stateMachine.Animator.SetBool(RArmOutHash, false);
        stateMachine.InputHandler.AttackEvent += OnAttack;
        stateMachine.InputHandler.DashEvent += OnDash;
        stateMachine.InputHandler.MeleeEvent += OnMelee;
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.InputHandler.AttackEvent -= OnAttack;
        stateMachine.InputHandler.DashEvent -= OnDash;
        stateMachine.InputHandler.MeleeEvent -= OnMelee;
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        HandleMovement();
        UpdateAnimationData();
        UpdateDirection();
    }

    private void HandleMovement()
    {
        if (stateMachine.InputHandler.lookInput)
        {
            stateMachine.InputHandler.OnMovement?.Invoke(Vector3.ClampMagnitude(stateMachine.InputHandler.MovementValue, 0.3f));
        }
        else
        {
            if (stateMachine.AgentMovement.isSprinting)
            {
                stateMachine.InputHandler.OnMovement?.Invoke(stateMachine.InputHandler.MovementValue.normalized);
            }
            else
            {
                stateMachine.InputHandler.OnMovement?.Invoke(stateMachine.InputHandler.MovementValue);
            }
        }
    }

    private void OnMelee(bool enabled)
    {
        if (enabled == true)
        {
            stateMachine.SwitchState(new PlayerMeleeState(stateMachine));
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
