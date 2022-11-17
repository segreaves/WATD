using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeState : PlayerMovementStateBase
{
    public PlayerMeleeState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        stateMachine.Animator.SetBool(RArmOutHash, true);
        stateMachine.InputReceiver.MeleeEvent += OnMelee;
        // Right arm layer
        stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("ArmR"), 0.7f);
        stateMachine.MeleeWeaponHandler.AttachToHand();
        stateMachine.Animator.CrossFadeInFixedTime(stateMachine.MeleeWeaponHandler.currentMelee.weaponData.WeaponName + "Equip", 0.0f, LayerMask.NameToLayer("ArmR"));
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.InputReceiver.MeleeEvent -= OnMelee;
        stateMachine.MeleeWeaponHandler.AttachToHolster();
        stateMachine.Animator.CrossFadeInFixedTime(stateMachine.MeleeWeaponHandler.currentMelee.weaponData.WeaponName + "Unequip", 0.0f, LayerMask.NameToLayer("UpperBody"));
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        stateMachine.InputReceiver.OnMovement?.Invoke(stateMachine.InputReceiver.MovementValue);
        stateMachine.InputReceiver.OnWalk.Invoke(stateMachine.InputReceiver.lookInput);
        UpdateAnimationData();
        UpdateDirection();
    }

    private void OnMelee(bool enabled)
    {
        if (enabled == false)
        {
            stateMachine.SwitchState(new PlayerFreeMovementState(stateMachine));
        }
    }
}
