using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMovementStateBase : State
{
    protected PlayerMovementStateBase(PlayerStateMachine stateMachine) : base(stateMachine) {}

    protected readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    protected readonly int FacingAngleHash = Animator.StringToHash("FacingAngle");
    protected readonly int LookAngleHash = Animator.StringToHash("LookAngle");
    protected readonly int LookOffsetHash = Animator.StringToHash("LookOffset");
    protected readonly int LArmOutHash = Animator.StringToHash("ArmL");
    protected readonly int RArmOutHash = Animator.StringToHash("ArmR");
    protected Vector3 facingDirection;
    private float lookAngle;

    public override void Enter()
    {
        facingDirection = stateMachine.AgentMovement.LastForwardDirection;
        InitializeHeadAim();
    }

    public override void Exit()
    {
        FinalizeHeadAim();
        stateMachine.Animator.SetBool(IsMovingHash, false);
    }

    public override void Tick(float deltaTime)
    {
        UpdateLastForwardDirection();
        UpdateLastLookDirection();
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
        if (stateMachine.InputHandler.movementInput == true)
        {
            if (stateMachine.InputHandler.lookInput == true)
            {
                stateMachine.AgentMovement.LastForwardDirection = stateMachine.InputHandler.LookValue.normalized;
            }
            else
            {
                stateMachine.AgentMovement.LastForwardDirection = stateMachine.InputHandler.MovementValue.normalized;
            }
        }
    }

    private void UpdateLastLookDirection()
    {
        if (stateMachine.InputHandler.movementInput == true)
        {
            if (stateMachine.InputHandler.lookInput == true)
            {
                stateMachine.AnimatorHandler.LookDirection = stateMachine.InputHandler.LookValue.normalized;
            }
            else
            {
                stateMachine.AnimatorHandler.LookDirection = stateMachine.InputHandler.MovementValue.normalized;
            }
        }
        else
        {
            if (stateMachine.InputHandler.lookInput == true)
            {
                stateMachine.AnimatorHandler.LookDirection = stateMachine.InputHandler.LookValue.normalized;
            }
        }
    }

    protected void UpdateAnimationData()
    {
        // Update look angle if not moving
        if (stateMachine.InputHandler.movementInput == false)
        {
            // Facing angle
            float facingAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.AgentMovement.LastForwardDirection);
            float facingAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.AgentMovement.LastForwardDirection) >= 0f ? -1f : 1f;
            if (facingAngleSign <= 0)
            {
                facingAngle = Math.Clamp(facingAngle, 0f, 180f) / 360f;
            }
            else
            {
                facingAngle = (360f - Math.Clamp(facingAngle, 0f, 180f)) / 360f;
            }
            stateMachine.Animator.SetFloat(FacingAngleHash, facingAngle, 0.0f, Time.deltaTime);
        }

        // Look angle
        lookAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.AnimatorHandler.LookDirection);
        float lookAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.AnimatorHandler.LookDirection) >= 0f ? 1f : -1f;
        float updatelookAngle = 0.5f + lookAngleSign * Math.Clamp(lookAngle, 0f, 90f) / 180f;
        stateMachine.Animator.SetFloat(LookAngleHash, updatelookAngle, 0.05f, Time.deltaTime);
        
        // Is moving
        stateMachine.Animator.SetBool(IsMovingHash, stateMachine.InputHandler.movementInput);
    }

    protected void UpdateDirection()
    {
        if (stateMachine.InputHandler.movementInput == true)
        {
            // Is moving
            if (stateMachine.InputHandler.lookInput == true)
            {
                // Look towards look input
                facingDirection = stateMachine.InputHandler.LookValue;
            }
            else
            {
                // Look towards movement velocity
                Vector3 velocity = stateMachine.InputHandler.Controller.velocity;
                velocity.y = 0f;
                if (velocity.sqrMagnitude > 0.1f)
                {
                    facingDirection = velocity.normalized;
                }
            }
            stateMachine.InputHandler.OnFaceDirection?.Invoke(facingDirection);
        }
        else
        {
            // Is not moving
            if (stateMachine.InputHandler.lookInput == true)
            {
                // Look towards look input
                float lookAngle = Vector3.Angle(stateMachine.AgentMovement.LastForwardDirection, stateMachine.InputHandler.LookValue);
                if (lookAngle >= 90f)
                {
                    float lookAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.InputHandler.LookValue) >= 0f ? 1f : -1f;
                    if (lookAngleSign >= 0f)
                    {
                        stateMachine.AgentMovement.LastForwardDirection = Quaternion.Euler(0, 120f, 0) * stateMachine.AgentMovement.LastForwardDirection;
                    }
                    else
                    {
                        stateMachine.AgentMovement.LastForwardDirection = Quaternion.Euler(0, -120f, 0) * stateMachine.AgentMovement.LastForwardDirection;
                    }
                }
                facingDirection = stateMachine.AgentMovement.LastForwardDirection;
            }
            stateMachine.InputHandler.OnRotateTowards.Invoke(facingDirection, 7f);
        }
    }
}
