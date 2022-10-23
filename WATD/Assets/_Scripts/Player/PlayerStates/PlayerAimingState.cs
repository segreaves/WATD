using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimingState : State
{
    public PlayerAimingState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    protected readonly int AimHash = Animator.StringToHash("Aiming");


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
}
