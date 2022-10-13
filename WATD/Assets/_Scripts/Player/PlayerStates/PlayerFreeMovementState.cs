using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeMovementState : State
{
    public PlayerFreeMovementState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        stateMachine.InputReceiver.DashEvent += OnDash;
        stateMachine.InputReceiver.AttackEvent += OnAttack;
    }

    public override void Exit()
    {
        stateMachine.InputReceiver.DashEvent -= OnDash;
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
    }

    public override void Tick(float deltaTime) {}

    private void OnDash()
    {
        stateMachine.SwitchState(new PlayerDashState(stateMachine));
    }

    private void OnAttack()
    {
        stateMachine.SwitchState(new PlayerAttackState(stateMachine));
    }
}
