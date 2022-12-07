using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : State
{
    public PlayerDashState(PlayerStateMachine stateMachine) : base(stateMachine) {}

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
        UpdateHeadAim();
        if (GetDashNormalizedTime() >= stateMachine.PlayerData.DashDuration)
        {
            stateMachine.SwitchState(new PlayerFreeMovementState(stateMachine));
        }
    }

    protected float GetDashNormalizedTime()
    {
        int layerIndex = stateMachine.Animator.GetLayerIndex("FullBody");
        AnimatorStateInfo currentInfo = stateMachine.Animator.GetCurrentAnimatorStateInfo(layerIndex);
        AnimatorStateInfo nextInfo = stateMachine.Animator.GetNextAnimatorStateInfo(layerIndex);
        if (stateMachine.Animator.IsInTransition(layerIndex) && nextInfo.IsTag("Dash"))
        {
            return nextInfo.normalizedTime;
        }
        else if (!stateMachine.Animator.IsInTransition(layerIndex) && currentInfo.IsTag("Dash"))
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f;
        }
    }

    private void InitializeHeadAim()
    {
        stateMachine.AnimatorHandler.LookDirection = stateMachine.transform.forward;
        stateMachine.AnimatorHandler.LookIKControl.StartLooking();
    }

    private void FinalizeHeadAim()
    {
        stateMachine.AnimatorHandler.LookIKControl.StopLooking();
    }

    private void UpdateHeadAim()
    {
        stateMachine.AnimatorHandler.LookIKControl.LookInDirection(stateMachine.transform.forward);
    }
}
