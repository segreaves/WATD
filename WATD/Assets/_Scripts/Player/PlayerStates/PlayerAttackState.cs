using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : State
{
    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    private int attackIndex;
    private WeaponSO currentWeaponData;
    private float attackTimer;

    public override void Enter()
    {
        attackIndex = stateMachine.WeaponHandler.attackIndex;
        stateMachine.WeaponHandler.IncrementAttackIndex();
        currentWeaponData = stateMachine.WeaponHandler.currentWeapon.weaponData;
        stateMachine.Animator.CrossFadeInFixedTime(currentWeaponData.AttackAnimations[attackIndex], currentWeaponData.TransitionDuration);
        stateMachine.InputReceiver.AttackEvent += OnAttack;
    }

    public override void Exit()
    {
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
    }

    public override void Tick(float deltaTime)
    {
        attackTimer += deltaTime;
        // Update direction
        if (attackTimer < currentWeaponData.RotationDuration)
        {
            if (stateMachine.InputReceiver.lookInput)
            {
                stateMachine.InputReceiver.OnRotateTowards?.Invoke(stateMachine.InputReceiver.LookValue, currentWeaponData.RotationSpeed);
            }
            else
            {
                Vector3 lookDirection = stateMachine.InputReceiver.Controller.velocity;
                lookDirection.y = 0f;
                stateMachine.InputReceiver.OnRotateTowards?.Invoke(lookDirection.normalized, currentWeaponData.RotationSpeed);
            }
        }
        // Start listening for attack event
        if (attackTimer > currentWeaponData.ComboStartTime)
        {
            //
        }
        // Check state exit
        if (attackTimer > currentWeaponData.AttackDuration)
        {
            stateMachine.SwitchState(new PlayerFreeMovementState(stateMachine));
        }
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

    private void OnAttack()
    {
        stateMachine.SwitchState(new PlayerAttackState(stateMachine));
    }
}
