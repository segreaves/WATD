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
        stateMachine.AnimatorHandler.LastBodyDirection = stateMachine.transform.forward;
        stateMachine.AnimatorHandler.LastLookDirection = stateMachine.transform.forward;
        stateMachine.AnimatorHandler.LookIKControl.StartLooking();
    }

    public override void Exit()
    {
        stateMachine.AnimatorHandler.LookIKControl.StopLooking();
        stateMachine.Animator.SetBool(stateMachine.AnimatorHandler.IsMovingHash, false);
    }

    public override void Tick(float deltaTime)
    {
        SetDirection();
        SetAnimationData();
    }

    private void SetDirection()
    {
        if (stateMachine.InputHandler.movementInput == true)
        {
            Vector3 bodyDirection;
            if (stateMachine.InputHandler.lookInput == true)
            {
                bodyDirection = stateMachine.InputHandler.LookValue.normalized;
            }
            else
            {
                bodyDirection = stateMachine.InputHandler.MovementValue.normalized;
            }
            stateMachine.InputHandler.OnFaceDirection?.Invoke(bodyDirection);
            stateMachine.AnimatorHandler.LastLookDirection = bodyDirection;
            stateMachine.AnimatorHandler.LastBodyDirection = bodyDirection;
            stateMachine.AnimatorHandler.LookIKControl.LookInDirection(bodyDirection);
        }
        else
        {
            if (stateMachine.InputHandler.lookInput == true)
            {
                // Look towards look input
                stateMachine.AnimatorHandler.LastLookDirection = stateMachine.InputHandler.LookValue.normalized;
                float lookAngle = Vector3.Angle(stateMachine.AnimatorHandler.LastBodyDirection, stateMachine.InputHandler.LookValue);
                if (lookAngle >= 90f)
                {
                    float lookAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.InputHandler.LookValue) >= 0f ? 1f : -1f;
                    if (lookAngleSign >= 0f)
                    {
                        stateMachine.AnimatorHandler.LastBodyDirection = Quaternion.Euler(0, 120f, 0) * stateMachine.AnimatorHandler.LastBodyDirection;
                    }
                    else
                    {
                        stateMachine.AnimatorHandler.LastBodyDirection = Quaternion.Euler(0, -120f, 0) * stateMachine.AnimatorHandler.LastBodyDirection;
                    }
                }
            }
            stateMachine.InputHandler.OnFaceDirection?.Invoke(stateMachine.AnimatorHandler.LastBodyDirection);
        }
    }

    private void SetAnimationData()
    {
        // Update look angle if not moving
        if (stateMachine.InputHandler.movementInput == false)
        {
            // Facing angle
            float facingAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.AnimatorHandler.LastBodyDirection);
            float facingAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.AnimatorHandler.LastBodyDirection) >= 0f ? -1f : 1f;
            if (facingAngleSign <= 0)
            {
                facingAngle = Math.Clamp(facingAngle, 0f, 180f) / 360f;
            }
            else
            {
                facingAngle = (360f - Math.Clamp(facingAngle, 0f, 180f)) / 360f;
            }
            stateMachine.Animator.SetFloat(stateMachine.AnimatorHandler.FacingAngleHash, facingAngle, 0.0f, Time.deltaTime);

            // Look angle
            float lookAngle;
            float lookAngleSign;
            if (stateMachine.InputHandler.lookInput == true)
            {
                lookAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.InputHandler.LookValue);
                lookAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.InputHandler.LookValue) >= 0f ? 1f : -1f;
                stateMachine.AnimatorHandler.LookIKControl.LookInDirection(stateMachine.InputHandler.LookValue);
            }
            else
            {
                lookAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.AnimatorHandler.LastLookDirection);
                lookAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.AnimatorHandler.LastLookDirection) >= 0f ? 1f : -1f;
                stateMachine.AnimatorHandler.LookIKControl.LookInDirection(stateMachine.AnimatorHandler.LastLookDirection);
            }
            float updatelookAngle = 0.5f + lookAngleSign * Math.Clamp(lookAngle, 0f, 90f) / 180f;
            stateMachine.Animator.SetFloat(stateMachine.AnimatorHandler.LookAngleHash, updatelookAngle, 0.025f, Time.deltaTime);
        }
        else
        {
            stateMachine.Animator.SetFloat(stateMachine.AnimatorHandler.LookAngleHash, 0.5f, 0.05f, Time.deltaTime);
        }
        
        // Is moving
        stateMachine.Animator.SetBool(stateMachine.AnimatorHandler.IsMovingHash, stateMachine.InputHandler.movementInput);
    }
}
