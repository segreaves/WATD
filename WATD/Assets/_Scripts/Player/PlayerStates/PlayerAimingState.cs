using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimingState : PlayerMovementStateBase
{
    public PlayerAimingState(PlayerStateMachine stateMachine, Vector3 lookDirection) : base(stateMachine)
    {
        stateMachine.AnimatorHandler.LastLookDirection = lookDirection;
    }

    protected RangedWeaponSO currentWeaponData;

    public override void Enter()
    {
        base.Enter();
        stateMachine.InputHandler.AimEvent += OnStopAiming;
        stateMachine.InputHandler.ShootEvent += OnShoot;
        //stateMachine.AnimatorHandler.LastLookDirection = stateMachine.transform.forward;
        currentWeaponData = stateMachine.RangedWeaponHandler.WeaponData;
        stateMachine.AnimatorHandler.animator.SetBool(stateMachine.AnimatorHandler.IsAimingHash, true);
        stateMachine.RangedWeaponHandler.AttachToHand();
        stateMachine.RangedWeaponHandler.StartAiming();
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.InputHandler.AimEvent -= OnStopAiming;
        stateMachine.InputHandler.ShootEvent -= OnShoot;
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

    private void OnShoot()
    {
        if (stateMachine.InputHandler.IsInteracting == true) { return; }
        bool shotSuccessful = stateMachine.RangedWeaponHandler.Shoot(stateMachine.AnimatorHandler.LastLookDirection, stateMachine.Power.power);
        if (shotSuccessful)
        {
            stateMachine.AnimatorHandler.PlayTargetAnimation(currentWeaponData.ShootAnimation, true, 0f);
        }
    }
}
