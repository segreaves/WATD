using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : State
{
    public PlayerDashState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    Vector3 dashDirection;
    private float rawTime;

    public override void Enter()
    {
        stateMachine.Animator.SetFloat(stateMachine.AnimatorHandler.LookAngleHash, 0.5f, 0f, Time.deltaTime);
        dashDirection = stateMachine.InputHandler.MovementValue.normalized;
        stateMachine.ForceReceiver?.ResetImpact();
        stateMachine.transform.rotation = Quaternion.LookRotation(dashDirection);
        stateMachine.AnimatorHandler.PlayTargetAnimation(stateMachine.AnimatorHandler.DashFHash, true, 0.1f);
        stateMachine.AgentMovement.LastForwardDirection = dashDirection;
    }

    public override void Exit()
    {
        stateMachine.InputHandler.StartCoroutine(stateMachine.InputHandler.EDashCooldown(stateMachine.PlayerData.DashCooldown));
        stateMachine.AnimatorHandler.animator.SetBool(stateMachine.AnimatorHandler.IsInteractingHash, false);
    }

    public override void Tick(float deltaTime)
    {
        rawTime += deltaTime;
        if (rawTime < stateMachine.PlayerData.DashInvulnerability)
        {
            stateMachine.AgentMovement.Move(dashDirection, stateMachine.PlayerData.DashImpulse);
        }
        else
        {
            stateMachine.AgentMovement.Move();
        }
        if (rawTime > stateMachine.PlayerData.DashDuration)
        {
            stateMachine.SwitchState(new PlayerFreeMovementState(stateMachine));
        }
    }
}
