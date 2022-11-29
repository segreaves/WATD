using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeMovementState_OLD : PlayerMovementStateBase
{
    public PlayerFreeMovementState_OLD(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        // Right arm layer
        stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("ArmR"), 1f);
        stateMachine.Animator.SetBool(LArmOutHash, false);
        stateMachine.Animator.SetBool(RArmOutHash, false);
        stateMachine.InputReceiver.AttackEvent += OnAttack;
        stateMachine.InputReceiver.DashEvent += OnDash;
        stateMachine.InputReceiver.MeleeEvent += OnMelee;
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
        stateMachine.InputReceiver.DashEvent -= OnDash;
        stateMachine.InputReceiver.MeleeEvent -= OnMelee;
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        stateMachine.InputReceiver.OnMovement?.Invoke(stateMachine.InputReceiver.MovementValue);
        stateMachine.InputReceiver.OnWalk.Invoke(stateMachine.InputReceiver.lookInput);
        UpdateAnimationData();
        UpdateDirection();
        stateMachine.Animator.SetFloat(GuardedFloatHash, 0f, 0.1f, Time.deltaTime);
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
        if (stateMachine.InputReceiver.MovementValue.sqrMagnitude > 0)
        {
            stateMachine.SwitchState(new PlayerDashState(stateMachine));
        }
    }

    private void OnAttack()
    {
        stateMachine.SwitchState(new PlayerMeleeEntryState(stateMachine));
    }
}
