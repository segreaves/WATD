using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeMovementState : State
{
    public PlayerFreeMovementState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    protected readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    protected readonly int AimHash = Animator.StringToHash("Aiming");
    Vector3 inputLookDirection;

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(LocomotionHash, 0.2f);
        stateMachine.InputReceiver.DashEvent += OnDash;
        stateMachine.InputReceiver.AttackEvent += OnAttack;
    }

    public override void Exit()
    {
        stateMachine.InputReceiver.DashEvent -= OnDash;
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
        stateMachine.Animator.SetBool(AimHash, false);
    }

    public override void Tick(float deltaTime)
    {
        // Update movement speed
        stateMachine.InputReceiver.OnWalk?.Invoke(stateMachine.InputReceiver.lookInput);
        // Update movement
        stateMachine.InputReceiver.OnMovement?.Invoke(stateMachine.InputReceiver.MovementValue);
        // Update direction
        UpdateFaceDirection();
        // Update animation
        UpdateAnimationData(deltaTime);
    }

    private void UpdateFaceDirection()
    {
        if (stateMachine.InputReceiver.lookInput == false && stateMachine.InputReceiver.movementInput == false)
        {
            // No movement or look input
            inputLookDirection = stateMachine.transform.forward;
        }
        else if (stateMachine.InputReceiver.lookInput == false && stateMachine.InputReceiver.movementInput == true)
        {
            // Face movement direction
            Vector3 velocity = stateMachine.InputReceiver.Controller.velocity;
            velocity.y = 0f;
            if (velocity.sqrMagnitude > 0.01f)
            {
                inputLookDirection = velocity.normalized;
            }
        }
        else if (stateMachine.InputReceiver.lookInput == true && stateMachine.InputReceiver.movementInput == true)
        {
            // Face look direction
            inputLookDirection = stateMachine.InputReceiver.LookValue;
        }
        else
        {
            // Face look direction if > 90 degrees from forward
            float angle = Quaternion.Angle(stateMachine.transform.rotation, Quaternion.LookRotation(stateMachine.InputReceiver.LookValue));
            if (angle >= 90)
            {
                inputLookDirection = stateMachine.InputReceiver.LookValue;
            }
        }
        stateMachine.InputReceiver.OnFaceDirection?.Invoke(inputLookDirection);
    }

    protected void UpdateAnimationData(float deltaTime)
    {
        // Aiming
        stateMachine.Animator.SetBool(AimHash, stateMachine.InputReceiver.aimEnabled);
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
        if (stateMachine.InputReceiver.aimEnabled == false)
        {
            stateMachine.SwitchState(new PlayerMeleeEntryState(stateMachine));
        }
        else
        {
            stateMachine.rangedWeapon.Shoot();
        }
    }
}
