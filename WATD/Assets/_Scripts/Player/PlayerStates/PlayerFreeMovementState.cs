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
        stateMachine.InputHandler.ShootEvent += OnShoot;
        stateMachine.InputHandler.DashEvent += OnDash;
        stateMachine.InputHandler.AimEvent += EnterRangeMode;
        if (stateMachine.InputHandler.IsAimingPressed)
        {
            EnterRangeMode(true);
        }
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.InputHandler.AttackEvent -= OnAttack;
        stateMachine.InputHandler.ShootEvent -= OnShoot;
        stateMachine.InputHandler.DashEvent -= OnDash;
        stateMachine.InputHandler.AimEvent -= EnterRangeMode;
        EnterRangeMode(false);
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

    private void OnShoot(bool shoot)
    {
        if (stateMachine.InputHandler.IsInteracting == true) { return; }
        stateMachine.RangedWeaponHandler.SetShooting(shoot);
    }

    private void EnterRangeMode(bool enable)
    {
        stateMachine.AnimatorHandler.animator.SetBool(stateMachine.AnimatorHandler.IsAimingHash, enable);
        if (enable)
        {
            stateMachine.RangedWeaponHandler.AttachToHand();
            stateMachine.RangedWeaponHandler.StartAiming();
        }
        else
        {
            //stateMachine.RangedWeaponHandler.SetShooting(false);
            stateMachine.RangedWeaponHandler.AttachToHolster();
            stateMachine.RangedWeaponHandler.StopAiming();
        }
    }
}
