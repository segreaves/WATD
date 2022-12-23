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
        /*float dashAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.InputHandler.MovementValue.normalized);
        if (dashAngle > 120f)
        {
            stateMachine.transform.rotation = Quaternion.LookRotation(-dashDirection);
            stateMachine.AnimatorHandler.PlayTargetAnimation(stateMachine.AnimatorHandler.DashBHash, true, 0.1f);
            stateMachine.AnimatorHandler.LastBodyDirection = -dashDirection;
        }
        else
        {
            stateMachine.transform.rotation = Quaternion.LookRotation(dashDirection);
            stateMachine.AnimatorHandler.PlayTargetAnimation(stateMachine.AnimatorHandler.DashFHash, true, 0.1f);
            stateMachine.AnimatorHandler.LastBodyDirection = dashDirection;
        }*/
        //stateMachine.ForceReceiver?.AddForce(dashDirection * stateMachine.PlayerData.DashImpulse);
        stateMachine.transform.rotation = Quaternion.LookRotation(dashDirection);
        stateMachine.AnimatorHandler.PlayTargetAnimation(stateMachine.AnimatorHandler.DashFHash, true, 0.1f);
        stateMachine.AnimatorHandler.LastBodyDirection = dashDirection;
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
        //stateMachine.AgentMovement.Move();
        if (GetDashNormalizedTime() >= 0.85f || (rawTime > stateMachine.PlayerData.DashDuration && stateMachine.InputHandler.movementInput == true))
        {
            stateMachine.SwitchState(new PlayerFreeMovementState(stateMachine));
        }
    }

    protected float GetDashNormalizedTime()
    {
        int layerIndex = stateMachine.Animator.GetLayerIndex("Full Body");
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
}
