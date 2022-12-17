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
        /*if (stateMachine.IsMeleeMode == true)
        {
            stateMachine.SwitchState(new PlayerMeleeEntryState(stateMachine));
        }
        else
        {
            EnterCombatMode();
        }*/
        stateMachine.SwitchState(new PlayerMeleeEntryState(stateMachine));
    }

    private void EnterCombatMode()
    {
        stateMachine.IsMeleeMode = true;
        stateMachine.MeleeWeaponHandler.AttachToHand();
        stateMachine.Animator.CrossFadeInFixedTime(stateMachine.MeleeWeaponHandler.currentMelee.weaponData.WeaponName + "InHand", 0.2f, LayerMask.NameToLayer("ArmR_Additive"));
        stateMachine.Animator.CrossFadeInFixedTime(stateMachine.MeleeWeaponHandler.currentMelee.weaponData.WeaponName + "Equip", 0.0f, LayerMask.NameToLayer("ArmR"));
    }
}
