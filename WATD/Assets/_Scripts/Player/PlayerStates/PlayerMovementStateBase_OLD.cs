using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementStateBase_OLD : State
{
    public PlayerMovementStateBase_OLD(PlayerStateMachine stateMachine) : base(stateMachine) {}

    protected Vector3 facingDirection;

    public override void Enter()
    {
        facingDirection = stateMachine.AnimatorHandler.LastBodyDirection;
        stateMachine.AnimatorHandler.LastLookDirection = stateMachine.transform.forward;
        // Initialize head look
        stateMachine.AnimatorHandler.LookIKControl.StartLooking();
    }

    public override void Exit()
    {
        FinalizeHeadAim();
        stateMachine.Animator.SetBool(stateMachine.AnimatorHandler.IsMovingHash, false);
    }

    public override void Tick(float deltaTime)
    {
        //UpdateLastForwardDirection();
        //UpdateLookDirection();
        //UpdateHeadAim();
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
            stateMachine.Animator.SetFloat(stateMachine.AnimatorHandler.LookAngleHash, updatelookAngle, 0.05f, Time.deltaTime);
        }
        else
        {
            stateMachine.Animator.SetFloat(stateMachine.AnimatorHandler.LookAngleHash, 0.5f, 0.05f, Time.deltaTime);
        }
        
        // Is moving
        stateMachine.Animator.SetBool(stateMachine.AnimatorHandler.IsMovingHash, stateMachine.InputHandler.movementInput);
    }

    private void InitializeHeadAim()
    {
        stateMachine.AnimatorHandler.LastLookDirection = stateMachine.transform.forward;
        stateMachine.AnimatorHandler.LookIKControl.StartLooking();
    }

    private void FinalizeHeadAim()
    {
        stateMachine.AnimatorHandler.LookIKControl.StopLooking();
    }

    private void UpdateHeadAim()
    {
        stateMachine.AnimatorHandler.LookIKControl.LookInDirection(stateMachine.AnimatorHandler.LastLookDirection);
    }

    private void UpdateLastForwardDirection()
    {
        if (stateMachine.AgentMovement.IsMoving == true)
        {
            if (stateMachine.InputHandler.lookInput == true)
            {
                stateMachine.AnimatorHandler.LastBodyDirection = stateMachine.InputHandler.LookValue.normalized;
            }
            else
            {
                stateMachine.AnimatorHandler.LastBodyDirection = stateMachine.transform.forward;
            }
        }
    }

    private void UpdateLookDirection()
    {
        if (stateMachine.AgentMovement.IsMoving == true)
        {
            stateMachine.AnimatorHandler.LastLookDirection = stateMachine.transform.forward;
        }
        else
        {
            if (stateMachine.InputHandler.lookInput == true)
            {
                stateMachine.AnimatorHandler.LastLookDirection = stateMachine.InputHandler.LookValue.normalized;
            }
        }
    }

    protected void UpdateAnimationData()
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
        }

        // Look angle
        float lookAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.AnimatorHandler.LastLookDirection);
        float lookAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.AnimatorHandler.LastLookDirection) >= 0f ? 1f : -1f;
        float updatelookAngle = 0.5f + lookAngleSign * Math.Clamp(lookAngle, 0f, 90f) / 180f;
        stateMachine.Animator.SetFloat(stateMachine.AnimatorHandler.LookAngleHash, updatelookAngle, 0.05f, Time.deltaTime);
        
        // Is moving
        stateMachine.Animator.SetBool(stateMachine.AnimatorHandler.IsMovingHash, stateMachine.InputHandler.movementInput);
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
                facingDirection = stateMachine.AnimatorHandler.LastBodyDirection;
            }
            stateMachine.InputHandler.OnRotateTowards.Invoke(facingDirection, 7f);
        }
    }
}
