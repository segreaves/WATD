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
        stateMachine.InputHandler.AimEvent += EnterRangeMode;
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.InputHandler.AttackEvent -= OnAttack;
        stateMachine.InputHandler.DashEvent -= OnDash;
        stateMachine.InputHandler.AimEvent -= EnterRangeMode;
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

    private void EnterRangeMode(bool enable)
    {
        stateMachine.AnimatorHandler.animator.SetBool(stateMachine.AnimatorHandler.IsAimingHash, enable);
        if (enable)
        {
            stateMachine.IsRangeMode = true;
            stateMachine.RangedWeaponHandler.AttachToHand();
            stateMachine.Animator.CrossFadeInFixedTime("AimF", 0.2f, LayerMask.NameToLayer("ArmR"));
            //stateMachine.Animator.CrossFadeInFixedTime(stateMachine.MeleeWeaponHandler.currentMelee.weaponData.WeaponName + "Equip", 0.0f, LayerMask.NameToLayer("ArmR"));
        }
        else
        {
            stateMachine.RangedWeaponHandler.AttachToHolster();
        }
    }
}
