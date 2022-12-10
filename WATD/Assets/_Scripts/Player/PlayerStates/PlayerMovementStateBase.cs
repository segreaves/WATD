using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMovementStateBase : State
{
    protected PlayerMovementStateBase(PlayerStateMachine stateMachine) : base(stateMachine) {}
    protected Vector3 facingDirection;

    public override void Enter()
    {
        facingDirection = stateMachine.AgentMovement.LastForwardDirection;
        InitializeHeadAim();
    }

    public override void Exit()
    {
        FinalizeHeadAim();
        stateMachine.Animator.SetBool(stateMachine.AnimatorHandler.IsMovingHash, false);
    }

    public override void Tick(float deltaTime)
    {
        UpdateLastForwardDirection();
        UpdateLookDirection();
        UpdateHeadAim();
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
        stateMachine.AnimatorHandler.LookIKControl.LookInDirection(stateMachine.AnimatorHandler.LookDirection);
    }

    private void UpdateLastForwardDirection()
    {
        if (stateMachine.AgentMovement.IsMoving == true)
        {
            if (stateMachine.InputHandler.lookInput == true)
            {
                stateMachine.AgentMovement.LastForwardDirection = stateMachine.InputHandler.LookValue.normalized;
            }
            else
            {
                stateMachine.AgentMovement.LastForwardDirection = stateMachine.transform.forward;
            }
        }
    }

    private void UpdateLookDirection()
    {
        if (stateMachine.AgentMovement.IsMoving == true)
        {
            stateMachine.AnimatorHandler.LookDirection = stateMachine.transform.forward;
        }
        else
        {
            if (stateMachine.InputHandler.lookInput == true)
            {
                stateMachine.AnimatorHandler.LookDirection = stateMachine.InputHandler.LookValue.normalized;
            }
        }
    }
}
