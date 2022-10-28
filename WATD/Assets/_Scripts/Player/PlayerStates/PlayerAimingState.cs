using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimingState : State
{
    public PlayerAimingState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    protected readonly int AimHash = Animator.StringToHash("Aiming");
    protected readonly int LookAngleHash = Animator.StringToHash("LookAngle");
    protected readonly int TurnLHash = Animator.StringToHash("TurnL");
    protected readonly int TurnRHash = Animator.StringToHash("TurnR");
    protected readonly int LookInputHash = Animator.StringToHash("LookInput");
    protected readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    public bool IsMoving => stateMachine.InputReceiver.MovementValue.sqrMagnitude > 0f;

    public override void Enter()
    {
        stateMachine.isMovementState = IsMovementState();
        stateMachine.Animator.SetBool(AimHash, true);
        stateMachine.InputReceiver.DashEvent += OnDash;
        stateMachine.InputReceiver.AimEvent += OnAim;
        stateMachine.WeaponHandler.ExtendCannon();
    }

    public override void Exit()
    {
        stateMachine.Animator.SetBool(AimHash, false);
        stateMachine.InputReceiver.DashEvent -= OnDash;
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
        stateMachine.InputReceiver.AimEvent -= OnAim;
        stateMachine.WeaponHandler.RetractCannon();
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.InputReceiver.OnWalk?.Invoke(stateMachine.InputReceiver.lookInput);
        stateMachine.InputReceiver.OnMovement?.Invoke(stateMachine.InputReceiver.MovementValue);
        UpdateFaceDirection();
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
        //
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
        // Look input
        stateMachine.Animator.SetBool(LookInputHash, stateMachine.InputReceiver.lookInput);
        // Look angle
        float lookAngle = stateMachine.InputReceiver.lookInput ? Quaternion.Angle(stateMachine.transform.rotation, Quaternion.LookRotation(stateMachine.InputReceiver.LookValue)) : 0f;
        float angleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.InputReceiver.LookValue) > 0 ? 1 : -1;
        float lookAngleNorm = Math.Clamp(angleSign * lookAngle, -180f, 180f) / 180f;
        lookAngleNorm = (1 + lookAngleNorm) / 2f;
        stateMachine.Animator.SetFloat(LookAngleHash, lookAngleNorm, 0.01f, Time.deltaTime);
        // Is moving
        stateMachine.Animator.SetBool(IsMovingHash, IsMoving);
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
