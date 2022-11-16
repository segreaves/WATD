using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeState : PlayerMovementStateBase
{
    public PlayerMeleeState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        stateMachine.Animator.SetBool(RArmOutHash, true);
        stateMachine.InputReceiver.MeleeEvent += OnMelee;
        // Right arm layer
        stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("ArmR"), 0.7f);
        stateMachine.MeleeWeaponHandler.AttachToHand();
        stateMachine.Animator.CrossFadeInFixedTime(stateMachine.MeleeWeaponHandler.currentMelee.weaponData.WeaponName + "Equip", 0.1f, LayerMask.NameToLayer("UpperBody"));
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.InputReceiver.MeleeEvent -= OnMelee;
        stateMachine.MeleeWeaponHandler.AttachToHolster();
        stateMachine.Animator.CrossFadeInFixedTime(stateMachine.MeleeWeaponHandler.currentMelee.weaponData.WeaponName + "Unequip", 0.1f, LayerMask.NameToLayer("UpperBody"));
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
