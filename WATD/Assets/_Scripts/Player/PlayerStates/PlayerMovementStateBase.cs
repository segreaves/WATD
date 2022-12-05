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
    protected readonly int GuardedBoolHash = Animator.StringToHash("Guarded_bool");
    protected readonly int GuardedFloatHash = Animator.StringToHash("Guarded_float");
    protected Vector3 facingDirection;
    private float lookAngle;

    public override void Enter()
    {
        facingDirection = stateMachine.AgentMovement.lastFacingDirection;
        stateMachine.Animator.SetBool(GuardedBoolHash, false);
        stateMachine.LookIKControl.StartLooking();
        //stateMachine.LockIkSwitcher.enabled = true;
        //stateMachine.LockIkSwitcher.LockFeet();
    }

    public override void Exit()
    {
        stateMachine.LookIKControl.StopLooking();
        //stateMachine.LockIkSwitcher.enabled = false;
    }

    public override void Tick(float deltaTime)
    {
        UpdateLastDirection();
        /*if (stateMachine.InputReceiver.movementInput == true)
        {
            stateMachine.LockIkSwitcher.UnlockFeet();
            stateMachine.LockIkSwitcher.UpdateFeet();
        }*/
        //stateMachine.LockIkSwitcher.lockLeftFoot = !stateMachine.AgentMovement.IsMoving;
        //stateMachine.LockIkSwitcher.lockRightFoot = !stateMachine.AgentMovement.IsMoving;
    }

    private void HeadAim()
    {
        if (stateMachine.InputReceiver.lookInput == true)
        {
            stateMachine.LookIKControl.LookInDirection(stateMachine.InputReceiver.LookValue);
        }
        else
        {
            stateMachine.LookIKControl.LookInDirection(stateMachine.transform.forward);
        }
    }

    private void UpdateLastDirection()
    {
        if (stateMachine.InputReceiver.movementInput == true)
        {
            if (stateMachine.InputReceiver.lookInput)
            {
                stateMachine.AgentMovement.SetLastDirection(stateMachine.InputReceiver.LookValue.normalized);
            }
            else
            {
                stateMachine.AgentMovement.SetLastDirection(stateMachine.InputReceiver.MovementValue.normalized);
            }
        }
    }

    protected void UpdateAnimationData()
    {
        // Update look angle if not moving
        if (stateMachine.InputReceiver.movementInput == false)
        {
            // Facing angle
            float facingAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.AgentMovement.lastFacingDirection);
            float facingAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.AgentMovement.lastFacingDirection) >= 0f ? -1f : 1f;
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
        lookAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.InputReceiver.LookValue);
        float lookAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.InputReceiver.LookValue) >= 0f ? 1f : -1f;
        float updatelookAngle = 0.5f + lookAngleSign * Math.Clamp(lookAngle, 0f, 90f) / 180f;
        stateMachine.Animator.SetFloat(LookAngleHash, updatelookAngle, 0.05f, Time.deltaTime);
        HeadAim();
        
        // Is moving
        stateMachine.Animator.SetBool(IsMovingHash, stateMachine.InputReceiver.movementInput);
    }

    protected void UpdateDirection()
    {
        if (stateMachine.InputReceiver.movementInput == true)
        {
            // Is moving
            if (stateMachine.InputReceiver.lookInput == true)
            {
                // Look towards look input
                facingDirection = stateMachine.InputReceiver.LookValue;
            }
            else
            {
                // Look towards movement velocity
                Vector3 velocity = stateMachine.InputReceiver.Controller.velocity;
                velocity.y = 0f;
                if (velocity.sqrMagnitude > 0.1f)
                {
                    facingDirection = velocity.normalized;
                }
            }
            stateMachine.InputReceiver.OnFaceDirection?.Invoke(facingDirection);
        }
        else
        {
            // Is not moving
            if (stateMachine.InputReceiver.lookInput == true)
            {
                // Look towards look input
                float lookAngle = Vector3.Angle(stateMachine.AgentMovement.lastFacingDirection, stateMachine.InputReceiver.LookValue);
                if (lookAngle >= 90f)
                {
                    float lookAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.InputReceiver.LookValue) >= 0f ? 1f : -1f;
                    if (lookAngleSign >= 0f)
                    {
                        stateMachine.AgentMovement.SetLastDirection(Quaternion.Euler(0, 120f, 0) * stateMachine.AgentMovement.lastFacingDirection);
                    }
                    else
                    {
                        stateMachine.AgentMovement.SetLastDirection(Quaternion.Euler(0, -120f, 0) * stateMachine.AgentMovement.lastFacingDirection);
                    }
                    //stateMachine.LockIkSwitcher.UnlockFeet();
                }
                facingDirection = stateMachine.AgentMovement.lastFacingDirection;
            }
            stateMachine.InputReceiver.OnRotateTowards.Invoke(facingDirection, 7f);
        }
    }
}
