using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatState : PlayerMovementStateBase
{
    public PlayerCombatState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        stateMachine.InputHandler.AttackEvent += OnAttack;
        stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("ArmR"), 0.75f);
        stateMachine.MeleeWeaponHandler.IgniteBlade();
        //stateMachine.Animator.CrossFadeInFixedTime(stateMachine.MeleeWeaponHandler.currentMelee.weaponData.WeaponName + "Equip", 0.0f, LayerMask.NameToLayer("ArmR"));
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.InputHandler.AttackEvent -= OnAttack;
        stateMachine.MeleeWeaponHandler.UnigniteBlade();
        //stateMachine.Animator.CrossFadeInFixedTime(stateMachine.MeleeWeaponHandler.currentMelee.weaponData.WeaponName + "Unequip", 0.0f, LayerMask.NameToLayer("UpperBody"));
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        stateMachine.InputHandler.OnMovement?.Invoke(stateMachine.InputHandler.MovementValue);
        //stateMachine.InputHandler.OnWalk.Invoke(stateMachine.InputHandler.lookInput);
    }

    private void OnAttack()
    {
        stateMachine.SwitchState(new PlayerMeleeEntryState(stateMachine));
    }
}
