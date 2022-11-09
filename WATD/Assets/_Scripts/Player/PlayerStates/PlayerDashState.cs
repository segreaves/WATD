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
        stateMachine.Animator.CrossFadeInFixedTime(dashHash, 0.1f);
        stateMachine.InputReceiver.AttackEvent += OnAttack;
        dashDirection = stateMachine.InputReceiver.MovementValue.normalized;
        stateMachine.ForceReceiver?.ResetImpact();
        stateMachine.ForceReceiver?.AddForce(dashDirection * stateMachine.PlayerData.DashImpulse);
    }

    public override void Exit()
    {
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
        stateMachine.InputReceiver.StartCoroutine(stateMachine.InputReceiver.EDashCooldown(stateMachine.PlayerData.DashCooldown));
        if (dashAttack)
        {
            // switch to dash attack state.
        }
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.AgentMovement.Move();
        stateMachine.AgentMovement.FaceDirection(dashDirection);
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
