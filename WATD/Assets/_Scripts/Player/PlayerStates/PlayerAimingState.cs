using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimingState : State
{
    public PlayerAimingState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    protected readonly int MovementHash = Animator.StringToHash("AimLocomotion");
    protected readonly int AimHash = Animator.StringToHash("Aiming");
    protected readonly int LookAngleHash = Animator.StringToHash("LookAngle");
    protected readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    protected readonly int TurnLHash = Animator.StringToHash("TurnL");
    protected readonly int TurnRHash = Animator.StringToHash("TurnR");
    private float lookingAngle;
    public bool IsMoving => stateMachine.InputReceiver.MovementValue.sqrMagnitude > 0f;

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(MovementHash, 0.1f);
        stateMachine.InputReceiver.OnWalk?.Invoke(true);
        stateMachine.isMovementState = IsMovementState();
        stateMachine.Animator.SetBool(AimHash, true);
        stateMachine.InputReceiver.DashEvent += OnDash;
        stateMachine.InputReceiver.AttackEvent += OnAttack;
        stateMachine.InputReceiver.AimEvent += OnAim;
        stateMachine.WeaponHandler.ExtendCannon();
        stateMachine.OnAim.Invoke(true);
    }

    public override void Exit()
    {
        stateMachine.InputReceiver.OnWalk?.Invoke(false);
        stateMachine.Animator.SetBool(AimHash, false);
        stateMachine.InputReceiver.DashEvent -= OnDash;
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
        stateMachine.InputReceiver.AimEvent -= OnAim;
        stateMachine.WeaponHandler.RetractCannon();
        stateMachine.OnAim.Invoke(false);
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.InputReceiver.OnMovement?.Invoke(stateMachine.InputReceiver.MovementValue);
        UpdateLookDirection();
        UpdateAnimationData();
        // Is moving
        stateMachine.Animator.SetBool(IsMovingHash, stateMachine.InputReceiver.movementInput);
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
        Debug.Log("Aiming state SHOOT");
    }

    private void OnAim(bool enabled)
    {
        if (enabled == false)
        {
            stateMachine.SwitchState(new PlayerFreeMovementState(stateMachine));
        }
    }

    protected override bool IsMovementState()
    {
        return true;
    }

    private void UpdateAnimationData()
    {
        // Look angle
        float lookAngle = stateMachine.InputReceiver.lookInput ? Quaternion.Angle(stateMachine.transform.rotation, Quaternion.LookRotation(stateMachine.InputReceiver.LookValue)) : 0f;
        float angleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.InputReceiver.LookValue) > 0f ? 1f : -1f;
        lookingAngle = angleSign * lookAngle;
        float lookAngleNorm = Math.Clamp(lookingAngle, -180f, 180f) / 180f;
        lookAngleNorm = (1f + lookAngleNorm) / 2f;
        stateMachine.Animator.SetFloat(LookAngleHash, lookAngleNorm, 0.02f, Time.deltaTime);
        // Is moving
        stateMachine.Animator.SetBool(IsMovingHash, stateMachine.InputReceiver.movementInput);
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
            // Face look direction if > 135 degrees from forward
            if (MathF.Abs(lookingAngle) >= 135f)
            {
                stateMachine.gameObject.transform.rotation = Quaternion.LookRotation(stateMachine.InputReceiver.LookValue);
                if (lookingAngle >= 0f)
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
}
