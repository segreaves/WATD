using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeMovementState : State
{
    public PlayerFreeMovementState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    protected readonly int MovementHash = Animator.StringToHash("Locomotion");
    protected readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    protected readonly int ArmsTuckedHash = Animator.StringToHash("ArmsTuckedIn");
    protected readonly int FacingAngleHash = Animator.StringToHash("FacingAngle");
    protected readonly int LookAngleHash = Animator.StringToHash("LookAngle");
    protected readonly int LookOffsetHash = Animator.StringToHash("LookOffset");
    private Vector3 facingDirection;
    private Vector3 lookDelay;
    private Vector3 dampVelocity;
    private float lookAngle;

    public override void Enter()
    {
        // If aim is pressed then switch immediately to aiming state
        if (stateMachine.InputReceiver.aimEnabled)
        {
            stateMachine.SwitchState(new PlayerAimingState(stateMachine));
        }
        stateMachine.Animator.SetBool(ArmsTuckedHash, true);
        stateMachine.InputReceiver.AttackEvent += OnAttack;
        stateMachine.InputReceiver.DashEvent += OnDash;
        stateMachine.InputReceiver.AimEvent += OnAim;
        facingDirection = stateMachine.AgentMovement.lastDirection;
        stateMachine.lookIKControl.StartLooking();
    }

    public override void Exit()
    {
        stateMachine.Animator.SetBool(ArmsTuckedHash, false);
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
        stateMachine.InputReceiver.DashEvent -= OnDash;
        stateMachine.InputReceiver.AimEvent -= OnAim;
        stateMachine.lookIKControl.StopLooking();
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.InputReceiver.OnMovement?.Invoke(stateMachine.InputReceiver.MovementValue);
        UpdateAnimationData();
        UpdateDirection();
        //stateMachine.InputReceiver.OnWalk.Invoke(stateMachine.InputReceiver.lookInput);
        if (stateMachine.InputReceiver.lookInput == true && lookAngle <= 135f)
        {
            stateMachine.InputReceiver.OnLookAt.Invoke(stateMachine.transform.position + Vector3.up * 0.5f + stateMachine.InputReceiver.LookValue.normalized * 2f);
        }
        else
        {
            stateMachine.InputReceiver.OnLookAt.Invoke(stateMachine.transform.position + Vector3.up * 0.5f + stateMachine.transform.forward * 2f);
        }
    }

    private void OnDash()
    {
        if (stateMachine.InputReceiver.MovementValue.sqrMagnitude > 0)
        {
            stateMachine.SwitchState(new PlayerDashState(stateMachine));
        }
    }

    private void OnAttack()
    {
        stateMachine.SwitchState(new PlayerMeleeEntryState(stateMachine));
    }

    private void OnAim(bool enabled)
    {
        if (enabled)
        {
            stateMachine.SwitchState(new PlayerAimingState(stateMachine));
        }
    }

    protected void UpdateDirection()
    {
        if (stateMachine.InputReceiver.movementInput == true)
        {
            // Look towards movement velocity
            Vector3 velocity = stateMachine.InputReceiver.Controller.velocity;
            velocity.y = 0f;
            if (velocity.sqrMagnitude > 0.01f)
            {
                facingDirection = velocity.normalized;
            }
            stateMachine.InputReceiver.OnFaceDirection?.Invoke(facingDirection);
        }
        else
        {
            // Is not moving
            if (stateMachine.InputReceiver.lookInput == true)
            {
                // Look towards look input
                float lookAngle = Vector3.Angle(stateMachine.AgentMovement.lastDirection, stateMachine.InputReceiver.LookValue);
                if (lookAngle >= 135f)
                {
                    float lookAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.InputReceiver.LookValue) >= 0f ? 1f : -1f;
                    if (lookAngleSign >= 0f)
                    {
                        stateMachine.AgentMovement.ResetLastDirection(Quaternion.Euler(0, 135f, 0) * stateMachine.AgentMovement.lastDirection);
                    }
                    else
                    {
                        stateMachine.AgentMovement.ResetLastDirection(Quaternion.Euler(0, -135f, 0) * stateMachine.AgentMovement.lastDirection);
                    }
                }
                facingDirection = stateMachine.AgentMovement.lastDirection;
            }
            stateMachine.InputReceiver.OnRotateTowards.Invoke(facingDirection, 10f);
        }
    }

    private void UpdateAnimationData()
    {
        // Facing angle
        float facingAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.AgentMovement.lastDirection);
        float facingAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.AgentMovement.lastDirection) >= 0f ? -1f : 1f;
        if (facingAngleSign <= 0)
        {
            facingAngle = Math.Clamp(facingAngle, 0f, 180f) / 360f;
        }
        else
        {
            facingAngle = (360f - Math.Clamp(facingAngle, 0f, 180f)) / 360f;
        }
        // Update look angle if not moving
        if (stateMachine.InputReceiver.movementInput == false)
        {
            stateMachine.Animator.SetFloat(FacingAngleHash, facingAngle, 0.0f, Time.deltaTime);
        }

        // Look angle
        lookAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.InputReceiver.LookValue);
        float lookAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.InputReceiver.LookValue) >= 0f ? 1f : -1f;
        float updatelookAngle = 0.5f + lookAngleSign * Math.Clamp(lookAngle, 0f, 180f) / 360f;
        stateMachine.Animator.SetFloat(LookAngleHash, updatelookAngle, 0.025f, Time.deltaTime);
        // Set weight of torso/head layer
        stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("Torso"), 1f - Mathf.Clamp01(stateMachine.InputReceiver.MovementValue.sqrMagnitude));
        // Look offset
        /*lookDelay = Vector3.SmoothDamp(lookDelay, stateMachine.transform.forward, ref dampVelocity, 0.2f);
        if (stateMachine.InputReceiver.movementInput == true)
        {
            stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("Crouch"), 0f);
        }
        else
        {
            float lookOffset = Quaternion.Angle(stateMachine.transform.rotation, Quaternion.LookRotation(lookDelay));
            lookOffset = Math.Clamp(lookOffset, 0f, 180f) / 180f;
            stateMachine.Animator.SetFloat(LookOffsetHash, lookOffset, 0.0f, Time.deltaTime);
            stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("Crouch"), lookOffset);
        }*/
        // Is moving
        stateMachine.Animator.SetBool(IsMovingHash, stateMachine.InputReceiver.movementInput);
    }
}
