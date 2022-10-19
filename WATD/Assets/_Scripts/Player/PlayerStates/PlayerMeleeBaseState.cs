using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeBaseState : State
{
    public PlayerMeleeBaseState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    protected bool isListeningForEvents = false;
    protected bool shouldCombo = false;
    protected bool shouldDash = false;
    protected int attackIndex;
    protected WeaponSO currentWeaponData;
    protected float attackTimer;

    public override void Enter()
    {
        if (stateMachine.WeaponHandler.weaponEnabled == false)
        {
            stateMachine.WeaponHandler.WeaponOn();
        }
        attackIndex = stateMachine.WeaponHandler.attackIndex;
        stateMachine.WeaponHandler.IncrementAttackIndex();
        currentWeaponData = stateMachine.WeaponHandler.currentWeapon.weaponData;
        stateMachine.Animator.CrossFadeInFixedTime(currentWeaponData.AttackAnimations[attackIndex], currentWeaponData.TransitionDuration);
        stateMachine.ForceReceiver.AttackImpulseEvent += OnAttackImpulse;
    }

    public override void Exit()
    {
        if (stateMachine.WeaponHandler.weaponEnabled == true)
        {
            stateMachine.WeaponHandler.WeaponOff();
        }
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
        stateMachine.InputReceiver.DashEvent -= OnDash;
        stateMachine.ForceReceiver.AttackImpulseEvent -= OnAttackImpulse;
    }

    public override void Tick(float deltaTime)
    {
        stateMachine.AgentMovement.Move();
        attackTimer += deltaTime;
        // Update direction
        if (attackTimer < currentWeaponData.RotationDuration)
        {
            if (stateMachine.InputReceiver.lookInput)
            {
                stateMachine.InputReceiver.OnRotateTowards?.Invoke(stateMachine.InputReceiver.LookValue, currentWeaponData.RotationSpeed);
            }
            else if (stateMachine.InputReceiver.MovementValue.sqrMagnitude >= 0.01f)
            {
                stateMachine.InputReceiver.OnRotateTowards?.Invoke(stateMachine.InputReceiver.MovementValue, currentWeaponData.RotationSpeed);
            }
            else
            {
                Vector3 lookDirection = stateMachine.InputReceiver.Controller.velocity;
                lookDirection.y = 0f;
                stateMachine.InputReceiver.OnRotateTowards?.Invoke(lookDirection.normalized, currentWeaponData.RotationSpeed);
            }
        }
        // Start listening for events
        if (attackTimer > currentWeaponData.ComboStartTime && !isListeningForEvents)
        {
            stateMachine.InputReceiver.AttackEvent += OnAttack;
            stateMachine.InputReceiver.DashEvent += OnDash;
            isListeningForEvents = true;
        }
    }

    protected void ExitConditions()
    {
        // Exit if is moving after MaxDuration
        if (attackTimer > currentWeaponData.MaxDuration && stateMachine.InputReceiver.MovementValue.magnitude > 0.1f)
        {
            stateMachine.SwitchState(new PlayerFreeMovementState(stateMachine));
        }
        // Exit if animation has finished
        if (GetAttackNormalizedTime() >= 1f)
        {
            stateMachine.SwitchState(new PlayerFreeMovementState(stateMachine));
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
        AnimatorStateInfo currentInfo = stateMachine.Animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo = stateMachine.Animator.GetNextAnimatorStateInfo(0);
        if (stateMachine.Animator.IsInTransition(0) && nextInfo.IsTag("Attack"))
        {
            return nextInfo.normalizedTime;
        }
        else if (!stateMachine.Animator.IsInTransition(0) && currentInfo.IsTag("Attack"))
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f;
        }
    }

    private void OnAttackImpulse()
    {
        stateMachine.ForceReceiver.AddFwdForce(5f);
    }
}
