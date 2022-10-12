using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeMovementState : State
{
    public PlayerFreeMovementState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        stateMachine.InputReceiver.movementEnabled = true;
        stateMachine.InputReceiver.rotationEnabled = true;
        stateMachine.InputReceiver.DashEvent += OnDash;
    }

    public override void Exit()
    {
        stateMachine.InputReceiver.DashEvent -= OnDash;
    }

    public override void Tick(float deltaTime) {}

    private void OnDash()
    {
        stateMachine.SwitchState(new PlayerDashState(stateMachine));
    }
}
