using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeMovementState : PlayerMovementStateBase
{
    public PlayerFreeMovementState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    private float lookAngle;

    public override void Enter()
    {
        base.Enter();
        //stateMachine.AnimatorHandler.animator.CrossFadeInFixedTime(stateMachine.AnimatorHandler.LocomotionHash, 0f, LayerMask.NameToLayer("Movement"));
        //stateMachine.AnimatorHandler.animator.CrossFadeInFixedTime(stateMachine.AnimatorHandler.DefaultHash, 0f, LayerMask.NameToLayer("Full Body"));
        stateMachine.AnimatorHandler.animator.SetFloat(stateMachine.AnimatorHandler.FacingAngleHash, 0f);
        stateMachine.InputHandler.AttackEvent += OnAttack;
        stateMachine.InputHandler.DashEvent += OnDash;
    }

    public override void Exit()
    {
        base.Exit();
        //stateMachine.AnimatorHandler.animator.CrossFadeInFixedTime("Default", 0f, LayerMask.NameToLayer("Fullbody"));
        stateMachine.InputHandler.AttackEvent -= OnAttack;
        stateMachine.InputHandler.DashEvent -= OnDash;
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        stateMachine.InputHandler.OnMovement?.Invoke(stateMachine.InputHandler.MovementValue);
        SetDirection();
        SetAnimationData();
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

    private void EnableMelee()
    {
        //meleeEnabled = true;
        // Draw sword
        //stateMachine.Animator.SetBool(stateMachine.AnimatorHandler.RArmOutHash, true);
        //stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("ArmR"), 0.7f);
        //stateMachine.MeleeWeaponHandler.AttachToHand();
        //stateMachine.Animator.CrossFadeInFixedTime(stateMachine.MeleeWeaponHandler.currentMelee.weaponData.WeaponName + "Equip", 0.0f, LayerMask.NameToLayer("ArmR"));
    }

    private void DisableMelee()
    {
        //meleeEnabled = false;
        //stateMachine.MeleeWeaponHandler.AttachToHolster();
        //stateMachine.Animator.CrossFadeInFixedTime(stateMachine.MeleeWeaponHandler.currentMelee.weaponData.WeaponName + "Unequip", 0.0f, LayerMask.NameToLayer("UpperBody"));
    }

    protected void SetDirection()
    {
        if (stateMachine.InputHandler.movementInput == true)
        {
            // Is moving
            if (stateMachine.InputHandler.lookInput == true)
            {
                // Look towards look input
                //facingDirection = stateMachine.InputHandler.LookValue;
                stateMachine.InputHandler.OnFaceDirection?.Invoke(stateMachine.InputHandler.LookValue);
            }
            else
            {
                // Look towards movement velocity
                Vector3 velocity = stateMachine.InputHandler.Controller.velocity;
                velocity.y = 0f;
                if (velocity.sqrMagnitude > 0.1f)
                {
                    //facingDirection = velocity.normalized;
                    stateMachine.InputHandler.OnFaceDirection?.Invoke(velocity.normalized);
                }
            }
            //stateMachine.InputHandler.OnFaceDirection?.Invoke(facingDirection);
        }
    }

    protected void SetAnimationData()
    {
        // Look angle
        if (stateMachine.AgentMovement.IsMoving == true)
        {
            stateMachine.Animator.SetFloat(stateMachine.AnimatorHandler.LookAngleHash, 0.5f, 0.1f, Time.deltaTime);
        }
        else
        {
            lookAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.AnimatorHandler.LookDirection);
            float lookAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.AnimatorHandler.LookDirection) >= 0f ? 1f : -1f;
            float updatelookAngle = 0.5f + lookAngleSign * Math.Clamp(lookAngle, 0f, 180f) / 360f;
            stateMachine.Animator.SetFloat(stateMachine.AnimatorHandler.LookAngleHash, updatelookAngle, 0.1f, Time.deltaTime);
        }
        
        // Is moving
        stateMachine.Animator.SetBool(stateMachine.AnimatorHandler.IsMovingHash, stateMachine.InputHandler.movementInput);
    }
}
