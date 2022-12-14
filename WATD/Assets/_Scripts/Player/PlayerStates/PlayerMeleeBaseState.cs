using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMeleeBaseState : State
{
    public PlayerMeleeBaseState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    protected bool isListeningForEvents = false;
    protected bool shouldCombo = false;
    protected bool shouldDash = false;
    protected int attackIndex;
    protected MeleeWeaponSO currentWeaponData;
    protected float attackTimer;

    public override void Enter()
    {
        stateMachine.MeleeWeaponHandler.IgniteBlade();
        attackIndex = stateMachine.MeleeWeaponHandler.attackIndex;
        stateMachine.MeleeWeaponHandler.IncrementAttackIndex();
        currentWeaponData = stateMachine.MeleeWeaponHandler.currentMelee.weaponData;
        stateMachine.AnimatorHandler.PlayTargetAnimation(currentWeaponData.AttackAnimations[attackIndex], true, 0f);
    }

    public override void Exit()
    {
        stateMachine.InputHandler.AttackEvent -= OnAttack;
        stateMachine.InputHandler.DashEvent -= OnDash;
        stateMachine.AnimatorHandler.animator.SetBool(stateMachine.AnimatorHandler.IsInteractingHash, false);
        if (shouldCombo == false)
        {
            stateMachine.MeleeWeaponHandler.UnigniteBlade();
        }
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.AgentMovement.Move();
        attackTimer += deltaTime;
        // Update direction
        if (attackTimer < currentWeaponData.RotationDuration)
        {
            if (stateMachine.InputHandler.lookInput)
            {
                stateMachine.InputHandler.OnRotateTowards?.Invoke(stateMachine.InputHandler.LookValue, currentWeaponData.RotationSpeed);
            }
            else if (stateMachine.InputHandler.MovementValue.sqrMagnitude >= 0.01f)
            {
                stateMachine.InputHandler.OnRotateTowards?.Invoke(stateMachine.InputHandler.MovementValue, currentWeaponData.RotationSpeed);
            }
            else
            {
                Vector3 lookDirection = stateMachine.gameObject.transform.forward;
                lookDirection.y = 0f;
                stateMachine.InputHandler.OnRotateTowards?.Invoke(stateMachine.AnimatorHandler.LastLookDirection, currentWeaponData.RotationSpeed);
            }
        }
        ExitConditions();
    }

    protected virtual void StartListeningForEvents()
    {
        stateMachine.InputHandler.AttackEvent += OnAttack;
        stateMachine.InputHandler.DashEvent += OnDash;
        isListeningForEvents = true;
    }

    protected void ExitConditions()
    {
        // Exit if animation has finished
        // Exit if is moving after MaxDuration
        if (GetAttackNormalizedTime() >= 1f || (attackTimer > currentWeaponData.AttackDuration + currentWeaponData.Cooldown && (stateMachine.InputHandler.movementInput == true || stateMachine.InputHandler.IsAimingPressed == true)))
        {
            stateMachine.SwitchState(new PlayerFreeMovementState(stateMachine));
        }
        if (attackTimer > currentWeaponData.AttackDuration + currentWeaponData.Cooldown)
        {
            stateMachine.MeleeWeaponHandler.UnigniteBlade();
        }
    }

    private void OnAttack()
    {
        shouldDash = false;
        shouldCombo = true;
    }

    private void OnDash()
    {
        shouldCombo = false;
        shouldDash = true;
    }

    protected float GetAttackNormalizedTime()
    {
        int layerIndex = stateMachine.Animator.GetLayerIndex("Full Body");
        AnimatorStateInfo currentInfo = stateMachine.Animator.GetCurrentAnimatorStateInfo(layerIndex);
        AnimatorStateInfo nextInfo = stateMachine.Animator.GetNextAnimatorStateInfo(layerIndex);
        if (stateMachine.Animator.IsInTransition(layerIndex) && nextInfo.IsTag("Attack"))
        {
            return nextInfo.normalizedTime;
        }
        else if (!stateMachine.Animator.IsInTransition(layerIndex) && currentInfo.IsTag("Attack"))
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f;
        }
    }
}
