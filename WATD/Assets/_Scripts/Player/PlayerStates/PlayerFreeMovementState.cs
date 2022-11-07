using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeMovementState : State
{
    public PlayerFreeMovementState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    protected readonly int MovementHash = Animator.StringToHash("Locomotion");
    protected readonly int ArmsTuckedHash = Animator.StringToHash("ArmsTuckedIn");
    protected readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    protected readonly int LookAngleHash = Animator.StringToHash("LookAngle");
    protected readonly int LookOffsetHash = Animator.StringToHash("LookOffset");
    private Vector3 desiredDirection;
    private Vector3 lookDelay;
    private Vector3 dampVelocity;

    public override void Enter()
    {
        // If aim is pressed then switch immediately to aiming state
        if (stateMachine.InputReceiver.aimEnabled)
        {
            stateMachine.SwitchState(new PlayerAimingState(stateMachine));
        }
        if (stateMachine.isMovementState == false)
        {
            stateMachine.Animator.CrossFadeInFixedTime(MovementHash, 0.1f);
        }
        stateMachine.Animator.SetBool(ArmsTuckedHash, true);
        stateMachine.isMovementState = IsMovementState();
        stateMachine.InputReceiver.AttackEvent += OnAttack;
        stateMachine.InputReceiver.DashEvent += OnDash;
        stateMachine.InputReceiver.AimEvent += OnAim;
    }

    public override void Exit()
    {
        stateMachine.Animator.SetBool(ArmsTuckedHash, false);
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
        stateMachine.InputReceiver.DashEvent -= OnDash;
        stateMachine.InputReceiver.AimEvent -= OnAim;
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.InputReceiver.OnWalk?.Invoke(stateMachine.InputReceiver.lookInput);
        stateMachine.InputReceiver.OnMovement?.Invoke(stateMachine.InputReceiver.MovementValue);
        UpdateDirection();
        UpdateAnimationData();
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
        if (enabled == true)
        {
            stateMachine.SwitchState(new PlayerAimingState(stateMachine));
        }
    }

    protected override bool IsMovementState()
    {
        return true;
    }

    protected void UpdateDirection()
    {
        if (stateMachine.InputReceiver.movementInput == true)
        {
            // If is moving
            if (stateMachine.InputReceiver.lookInput == true)
            {
                // Look towards look input
                desiredDirection = stateMachine.InputReceiver.LookValue;
            }
            else
            {
                // Look towards movement velocity
                Vector3 velocity = stateMachine.InputReceiver.Controller.velocity;
                velocity.y = 0f;
                if (velocity.sqrMagnitude > 0.01f)
                {
                    desiredDirection = velocity.normalized;
                }
            }
        }
        else
        {
            // If is moving
            if (stateMachine.InputReceiver.lookInput == true)
            {
                // Look towards look input
                desiredDirection = stateMachine.InputReceiver.LookValue;
            }
        }
        stateMachine.InputReceiver.OnFaceDirection?.Invoke(desiredDirection);
    }

    private void UpdateAnimationData()
    {
        // Look angle
        float lookAngle = Quaternion.Angle(stateMachine.transform.rotation, Quaternion.LookRotation(stateMachine.AgentMovement.lastDirection));
        float angleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.AgentMovement.lastDirection) >= 0f ? -1f : 1f;
        if (angleSign <= 0)
        {
            lookAngle = Math.Clamp(lookAngle, 0f, 180f) / 360f;
        }
        else
        {
            lookAngle = (360f - Math.Clamp(lookAngle, 0f, 180f)) / 360f;
        }
        stateMachine.Animator.SetFloat(LookAngleHash, lookAngle, 0.0f, Time.deltaTime);
        // Look offset
        lookDelay = Vector3.SmoothDamp(lookDelay, stateMachine.transform.forward, ref dampVelocity, 0.2f);
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
        }
        // Is moving
        stateMachine.Animator.SetBool(IsMovingHash, stateMachine.InputReceiver.movementInput);
    }
}
