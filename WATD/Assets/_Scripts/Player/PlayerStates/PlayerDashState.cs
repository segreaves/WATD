using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : State
{
    public PlayerDashState(PlayerStateMachine stateMachine) : base(stateMachine) {}
    
    private float rawTime = 0f;

    public override void Enter()
    {
        Vector3 dashDirection = stateMachine.InputHandler.MovementValue.normalized;
        stateMachine.transform.rotation = Quaternion.LookRotation(dashDirection);
        stateMachine.AnimatorHandler.PlayTargetAnimation(stateMachine.AnimatorHandler.DashHash, false, 0.1f);
        stateMachine.ForceReceiver?.ResetImpact();
        stateMachine.ForceReceiver?.AddForce(dashDirection * stateMachine.PlayerData.DashImpulse);
    }

    public override void Exit()
    {
        stateMachine.InputHandler.StartCoroutine(stateMachine.InputHandler.EDashCooldown(stateMachine.PlayerData.DashCooldown));
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.AgentMovement.Move();
        rawTime += deltaTime;
        if (rawTime >= stateMachine.PlayerData.DashDuration)
        {
            stateMachine.SwitchState(new PlayerFreeMovementState(stateMachine));
        }
    }
}
