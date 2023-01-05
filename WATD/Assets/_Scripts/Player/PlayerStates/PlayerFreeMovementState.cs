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
        stateMachine.AnimatorHandler.animator.SetFloat(stateMachine.AnimatorHandler.FacingAngleHash, 0f);
        stateMachine.InputHandler.AttackEvent += OnAttack;
        stateMachine.InputHandler.DashEvent += OnDash;
        stateMachine.InputHandler.AimEvent += OnAim;
        if (stateMachine.InputHandler.IsAimingPressed)
        {
            stateMachine.SwitchState(new PlayerAimingState(stateMachine, stateMachine.AnimatorHandler.LastLookDirection));
        }
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.InputHandler.AttackEvent -= OnAttack;
        stateMachine.InputHandler.DashEvent -= OnDash;
        stateMachine.InputHandler.AimEvent -= OnAim;
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        stateMachine.InputHandler.OnMovement?.Invoke(stateMachine.InputHandler.MovementValue);
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

    private void OnAim(bool aim)
    {
        if (aim == false) { return; }
        stateMachine.SwitchState(new PlayerAimingState(stateMachine, stateMachine.AnimatorHandler.LastLookDirection));
    }
}
