using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeMovementState : State
{
    public PlayerFreeMovementState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    protected readonly int MovementHash = Animator.StringToHash("Locomotion");
    protected readonly int LookAngleHash = Animator.StringToHash("LookAngle");
    protected readonly int TurnLHash = Animator.StringToHash("TurnL");
    protected readonly int TurnRHash = Animator.StringToHash("TurnR");
    protected readonly int LookInputHash = Animator.StringToHash("LookInput");

    public override void Enter()
    {
        if (stateMachine.isMovementState == false)
        {
            stateMachine.Animator.CrossFadeInFixedTime(MovementHash, 0.2f);
        }
        stateMachine.isMovementState = IsMovementState();
        stateMachine.InputReceiver.AttackEvent += OnAttack;
        stateMachine.InputReceiver.DashEvent += OnDash;
        stateMachine.InputReceiver.AimEvent += OnAim;
        // If aim is pressed then switch immediately to aiming state
        if (stateMachine.InputReceiver.aimEnabled)
        {
            stateMachine.SwitchState(new PlayerAimingState(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
        stateMachine.InputReceiver.DashEvent -= OnDash;
        stateMachine.InputReceiver.AimEvent -= OnAim;
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.InputReceiver.OnWalk?.Invoke(stateMachine.InputReceiver.lookInput);
        stateMachine.InputReceiver.OnMovement?.Invoke(stateMachine.InputReceiver.MovementValue);
        UpdateLookDirection();
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

    private void UpdateAnimationData()
    {
        // Look input
        stateMachine.Animator.SetBool(LookInputHash, stateMachine.InputReceiver.lookInput);
        // Look angle
        float lookAngle = stateMachine.InputReceiver.lookInput ? Quaternion.Angle(stateMachine.transform.rotation, Quaternion.LookRotation(stateMachine.InputReceiver.LookValue)) : 0f;
        float angleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.InputReceiver.LookValue) > 0 ? 1 : -1;
        float lookAngleNorm = Math.Clamp(angleSign * lookAngle, -180f, 180f) / 180f;
        lookAngleNorm = (1 + lookAngleNorm) / 2f;
        stateMachine.Animator.SetFloat(LookAngleHash, lookAngleNorm, 0.025f, Time.deltaTime);
    }

    protected void UpdateLookDirection()
    {
        if (stateMachine.InputReceiver.lookInput == false && stateMachine.InputReceiver.movementInput == true)
        {
            // Face movement direction
            Vector3 velocity = stateMachine.InputReceiver.Controller.velocity;
            velocity.y = 0f;
            if (velocity.sqrMagnitude > 0.01f)
            {
                stateMachine.InputReceiver.OnFaceDirection?.Invoke(velocity.normalized);
            }
        }
        else if (stateMachine.InputReceiver.lookInput == true && stateMachine.InputReceiver.movementInput == true)
        {
            // Face look direction
            stateMachine.InputReceiver.OnFaceDirection?.Invoke(stateMachine.InputReceiver.LookValue);
        }
        else if (stateMachine.InputReceiver.lookInput == true && stateMachine.InputReceiver.movementInput == false)
        {
            // Face look direction if > 90 degrees from forward
            float angle = Quaternion.Angle(stateMachine.transform.rotation, Quaternion.LookRotation(stateMachine.InputReceiver.LookValue));
            bool right = Vector3.Dot(stateMachine.transform.right, stateMachine.InputReceiver.LookValue) > 0 ? true : false;
            if (angle >= 135f)
            {
                stateMachine.gameObject.transform.rotation = Quaternion.LookRotation(stateMachine.InputReceiver.LookValue);
                if (right)
                {
                    stateMachine.Animator.CrossFadeInFixedTime(TurnRHash, 0.0f);
                }
                else
                {
                    stateMachine.Animator.CrossFadeInFixedTime(TurnLHash, 0.0f);
                }
            }
        }
    }

    protected override void TurnL()
    {
        stateMachine.Animator.CrossFadeInFixedTime(TurnLHash, 0.05f);
    }

    protected override void TurnR()
    {
        stateMachine.Animator.CrossFadeInFixedTime(TurnRHash, 0.05f);
    }
}
