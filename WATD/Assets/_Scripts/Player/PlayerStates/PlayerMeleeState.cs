using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeState : PlayerMovementStateBase
{
    public PlayerMeleeState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        stateMachine.Animator.SetBool(stateMachine.AnimatorHandler.RArmOutHash, true);
        stateMachine.InputHandler.AttackEvent += OnAttack;
        // Right arm layer
        stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("ArmR"), 0.7f);
        stateMachine.MeleeWeaponHandler.AttachToHand();
        stateMachine.Animator.CrossFadeInFixedTime(stateMachine.MeleeWeaponHandler.currentMelee.weaponData.WeaponName + "Equip", 0.0f, LayerMask.NameToLayer("ArmR"));
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.InputHandler.AttackEvent -= OnAttack;
        stateMachine.MeleeWeaponHandler.AttachToHolster();
        stateMachine.Animator.CrossFadeInFixedTime(stateMachine.MeleeWeaponHandler.currentMelee.weaponData.WeaponName + "Unequip", 0.0f, LayerMask.NameToLayer("UpperBody"));
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        stateMachine.InputHandler.OnMovement?.Invoke(stateMachine.InputHandler.MovementValue);
        //stateMachine.InputHandler.OnWalk.Invoke(stateMachine.InputHandler.lookInput);
        //UpdateAnimationData();
        //UpdateDirection();
    }

    private void OnAttack()
    {
        stateMachine.SwitchState(new PlayerMeleeEntryState(stateMachine));
    }
}
