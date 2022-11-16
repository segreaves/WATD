using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeMovementState : PlayerMovementStateBase
{
    public PlayerFreeMovementState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        // Right arm layer
        stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("ArmR"), 1f);
        stateMachine.Animator.SetBool(LArmOutHash, false);
        stateMachine.Animator.SetBool(RArmOutHash, false);
        stateMachine.InputReceiver.AttackEvent += OnAttack;
        stateMachine.InputReceiver.DashEvent += OnDash;
        stateMachine.InputReceiver.MeleeEvent += OnMelee;
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
        stateMachine.InputReceiver.DashEvent -= OnDash;
        stateMachine.InputReceiver.MeleeEvent -= OnMelee;
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        stateMachine.InputReceiver.OnMovement?.Invoke(stateMachine.InputReceiver.MovementValue);
        stateMachine.InputReceiver.OnWalk.Invoke(stateMachine.InputReceiver.lookInput);
        UpdateAnimationData();
        UpdateDirection();
    }

    private void OnMelee(bool enabled)
    {
        if (enabled == true)
        {
            stateMachine.SwitchState(new PlayerMeleeState(stateMachine));
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
}
