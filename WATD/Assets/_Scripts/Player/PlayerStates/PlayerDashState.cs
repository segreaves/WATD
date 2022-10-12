using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : State
{
    public PlayerDashState(PlayerStateMachine stateMachine) : base(stateMachine) {}
    
    private float rawTime = 0f;
    private readonly int dashHash = Animator.StringToHash("Dash");
    private Vector3 dashDirection;
    private bool dashAttack = false;

    public override void Enter()
    {
        stateMachine.InputReceiver.AttackEvent += OnAttack;
        stateMachine.InputReceiver.DisableMovement();
        stateMachine.InputReceiver.DisableRotation();
        dashDirection = stateMachine.InputReceiver.MovementValue.normalized;
        if (stateMachine.ForceReceiver)
        {
            stateMachine.ForceReceiver.AddForce(dashDirection * stateMachine.PlayerData.DashImpulse);
        }
    }

    public override void Exit()
    {
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
        stateMachine.InputReceiver.StartCoroutine(stateMachine.InputReceiver.EDashCooldown(stateMachine.PlayerData.DashCooldown));
        if (dashAttack)
        {}
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.AgentMovement.Look(dashDirection);
        rawTime += deltaTime;
        if (rawTime >= stateMachine.PlayerData.DashDuration)
        {
            stateMachine.SwitchState(new PlayerFreeMovementState(stateMachine));
        }
    }

    private void OnAttack()
    {
        dashAttack = true;
    }
}
