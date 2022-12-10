using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimingState : PlayerMovementStateBase
{
    public PlayerAimingState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    private float lookAngle;

    public override void Enter()
    {
        base.Enter();
        // Right arm layer
        stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("ArmR"), 1f);
        stateMachine.Animator.SetBool(stateMachine.AnimatorHandler.LArmOutHash, false);
        stateMachine.Animator.SetBool(stateMachine.AnimatorHandler.RArmOutHash, false);
        stateMachine.InputHandler.AttackEvent += OnAttack;
        stateMachine.InputHandler.DashEvent += OnDash;
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.InputHandler.AttackEvent -= OnAttack;
        stateMachine.InputHandler.DashEvent -= OnDash;
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        HandleMovement();
        UpdateAnimationData();
        UpdateDirection();
    }

    private void HandleMovement()
    {
        if (stateMachine.InputHandler.lookInput)
        {
            stateMachine.InputHandler.OnMovement?.Invoke(Vector3.ClampMagnitude(stateMachine.InputHandler.MovementValue, 0.3f));
        }
        else
        {
            if (stateMachine.AgentMovement.isSprinting)
            {
                stateMachine.InputHandler.OnMovement?.Invoke(stateMachine.InputHandler.MovementValue.normalized);
            }
            else
            {
                stateMachine.InputHandler.OnMovement?.Invoke(stateMachine.InputHandler.MovementValue);
            }
        }
    }

    private void OnDash()
    {
        if (stateMachine.InputHandler.IsInteracting == true) { return; }
        if (stateMachine.InputHandler.MovementValue.sqrMagnitude <= 0) { return; }
        stateMachine.SwitchState(new PlayerDashState(stateMachine));
    }

    private void OnAttack()
    {
        if (stateMachine.InputHandler.IsInteracting == true) { return; }
        stateMachine.SwitchState(new PlayerMeleeEntryState(stateMachine));
    }

    private void UpdateAnimationData()
    {
        // Update look angle if not moving
        if (stateMachine.InputHandler.movementInput == false)
        {
            // Facing angle
            float facingAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.AgentMovement.LastForwardDirection);
            float facingAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.AgentMovement.LastForwardDirection) >= 0f ? -1f : 1f;
            if (facingAngleSign <= 0)
            {
                facingAngle = Math.Clamp(facingAngle, 0f, 180f) / 360f;
            }
            else
            {
                facingAngle = (360f - Math.Clamp(facingAngle, 0f, 180f)) / 360f;
            }
            stateMachine.Animator.SetFloat(stateMachine.AnimatorHandler.FacingAngleHash, facingAngle, 0.0f, Time.deltaTime);
        }

        // Look angle
        lookAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.AnimatorHandler.LookDirection);
        float lookAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.AnimatorHandler.LookDirection) >= 0f ? 1f : -1f;
        float updatelookAngle = 0.5f + lookAngleSign * Math.Clamp(lookAngle, 0f, 90f) / 180f;
        stateMachine.Animator.SetFloat(stateMachine.AnimatorHandler.LookAngleHash, updatelookAngle, 0.05f, Time.deltaTime);
        
        // Is moving
        stateMachine.Animator.SetBool(stateMachine.AnimatorHandler.IsMovingHash, stateMachine.InputHandler.movementInput);
    }

    private void UpdateDirection()
    {
        if (stateMachine.InputHandler.movementInput == true)
        {
            // Is moving
            if (stateMachine.InputHandler.lookInput == true)
            {
                // Look towards look input
                facingDirection = stateMachine.InputHandler.LookValue;
            }
            else
            {
                // Look towards movement velocity
                Vector3 velocity = stateMachine.InputHandler.Controller.velocity;
                velocity.y = 0f;
                if (velocity.sqrMagnitude > 0.1f)
                {
                    facingDirection = velocity.normalized;
                }
            }
            stateMachine.InputHandler.OnFaceDirection?.Invoke(facingDirection);
        }
        else
        {
            // Is not moving
            if (stateMachine.InputHandler.lookInput == true)
            {
                // Look towards look input
                float lookAngle = Vector3.Angle(stateMachine.AgentMovement.LastForwardDirection, stateMachine.InputHandler.LookValue);
                if (lookAngle >= 90f)
                {
                    float lookAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.InputHandler.LookValue) >= 0f ? 1f : -1f;
                    if (lookAngleSign >= 0f)
                    {
                        stateMachine.AgentMovement.LastForwardDirection = Quaternion.Euler(0, 120f, 0) * stateMachine.AgentMovement.LastForwardDirection;
                    }
                    else
                    {
                        stateMachine.AgentMovement.LastForwardDirection = Quaternion.Euler(0, -120f, 0) * stateMachine.AgentMovement.LastForwardDirection;
                    }
                }
                facingDirection = stateMachine.AgentMovement.LastForwardDirection;
            }
            stateMachine.InputHandler.OnRotateTowards.Invoke(facingDirection, 7f);
        }
    }
}
