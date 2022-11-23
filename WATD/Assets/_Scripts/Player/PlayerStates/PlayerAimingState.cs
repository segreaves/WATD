using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimingState : State
{
    public PlayerAimingState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    protected readonly int MovementHash = Animator.StringToHash("Locomotion");
    protected readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    protected readonly int ArmsTuckedHash = Animator.StringToHash("ArmsTuckedIn");
    protected readonly int LookAngleHash = Animator.StringToHash("LookAngle");
    protected readonly int LookOffsetHash = Animator.StringToHash("LookOffset");
    protected readonly int AimHash = Animator.StringToHash("Aim");
    private Vector3 facingDirection;
    private Vector3 lookDelay;
    private Vector3 dampVelocity;

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(AimHash, 0.1f);
        stateMachine.InputReceiver.OnWalk?.Invoke(true);
        //stateMachine.Animator.SetBool(AimHash, true);
        stateMachine.InputReceiver.DashEvent += OnDash;
        stateMachine.InputReceiver.AttackEvent += OnAttack;
        stateMachine.InputReceiver.AimEvent += OnAim;
        stateMachine.RangedWeaponHandler.AttachToHand();
    }

    public override void Exit()
    {
        stateMachine.InputReceiver.OnWalk?.Invoke(false);
        //stateMachine.Animator.SetBool(AimHash, false);
        stateMachine.InputReceiver.DashEvent -= OnDash;
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
        stateMachine.InputReceiver.AimEvent -= OnAim;
        stateMachine.RangedWeaponHandler.AttachToHolster();
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.InputReceiver.OnMovement?.Invoke(stateMachine.InputReceiver.MovementValue);
        UpdateDirection();
        //UpdateAnimationData();
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
                if (velocity.sqrMagnitude > 0.01f)
                {
                    facingDirection = velocity.normalized;
                }
            }
        }
        else
        {
            // Is not moving
            if (stateMachine.InputReceiver.lookInput == true)
            {
                // Look towards look input
                facingDirection = stateMachine.InputReceiver.LookValue;
            }
        }
        stateMachine.InputReceiver.OnFaceDirection?.Invoke(facingDirection);
    }

    /*private void UpdateAnimationData()
    {
        // Look angle
        float lookAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.AgentMovement.lastDirection);
        float angleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.AgentMovement.lastDirection) >= 0f ? -1f : 1f;
        if (angleSign <= 0)
        {
            lookAngle = Math.Clamp(lookAngle, 0f, 180f) / 360f;
        }
        else
        {
            lookAngle = (360f - Math.Clamp(lookAngle, 0f, 180f)) / 360f;
        }
        // Update look angle if not moving
        if (stateMachine.InputReceiver.movementInput == false)
        {
            stateMachine.Animator.SetFloat(LookAngleHash, lookAngle, 0.0f, Time.deltaTime);
        }
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
    }*/
}
