using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimingState : PlayerMovementStateBase
{
    public PlayerAimingState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        stateMachine.InputHandler.AimEvent += OnStopAiming;
        stateMachine.InputHandler.AttackEvent += OnAttack;
        stateMachine.AnimatorHandler.animator.SetBool(stateMachine.AnimatorHandler.IsAimingHash, true);
        stateMachine.RangedWeaponHandler.AttachToHand();
        stateMachine.RangedWeaponHandler.StartAiming();
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.InputHandler.AimEvent -= OnStopAiming;
        stateMachine.InputHandler.AttackEvent -= OnAttack;
        stateMachine.AnimatorHandler.animator.SetBool(stateMachine.AnimatorHandler.IsAimingHash, false);
        stateMachine.RangedWeaponHandler.AttachToHolster();
        stateMachine.RangedWeaponHandler.StopAiming();
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        stateMachine.InputHandler.OnMovement?.Invoke(stateMachine.InputHandler.MovementValue);
    }

    private void OnStopAiming(bool aim)
    {
        if (aim == true) { return; }
        stateMachine.SwitchState(new PlayerFreeMovementState(stateMachine));
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
        stateMachine.RangedWeaponHandler.Shoot(stateMachine.Power.power);
    }
}
