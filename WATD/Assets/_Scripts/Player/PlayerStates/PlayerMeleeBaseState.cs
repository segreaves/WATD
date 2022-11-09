using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMeleeBaseState : State
{
    public PlayerMeleeBaseState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    protected readonly int MovementHash = Animator.StringToHash("Locomotion");
    protected bool isListeningForEvents = false;
    protected bool shouldCombo = false;
    protected bool shouldDash = false;
    protected int attackIndex;
    protected MeleeWeaponSO currentWeaponData;
    protected float attackTimer;

    public override void Enter()
    {
        stateMachine.MeleeWeaponHandler.AttachToHand();
        attackIndex = stateMachine.MeleeWeaponHandler.attackIndex;
        stateMachine.MeleeWeaponHandler.IncrementAttackIndex();
        currentWeaponData = stateMachine.MeleeWeaponHandler.currentWeapon.weaponData;
        stateMachine.Animator.CrossFadeInFixedTime(currentWeaponData.AttackAnimations[attackIndex], 0.1f);
    }

    public override void Exit()
    {
        stateMachine.MeleeWeaponHandler.AttachToHolster();
        stateMachine.InputReceiver.AttackEvent -= OnAttack;
        stateMachine.InputReceiver.DashEvent -= OnDash;
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
                Vector3 lookDirection = stateMachine.gameObject.transform.forward;
                lookDirection.y = 0f;
                stateMachine.InputReceiver.OnRotateTowards?.Invoke(lookDirection.normalized, currentWeaponData.RotationSpeed);
            }
        }
        ExitConditions();
    }

    protected virtual void StartListeningForEvents()
    {
        stateMachine.InputReceiver.AttackEvent += OnAttack;
        stateMachine.InputReceiver.DashEvent += OnDash;
        isListeningForEvents = true;
    }

    protected void ExitConditions()
    {
        // Exit if is moving after MaxDuration
        if (attackTimer > currentWeaponData.AttackDuration + currentWeaponData.Cooldown && stateMachine.InputReceiver.MovementValue.magnitude > 0.1f)
        {
            stateMachine.SwitchState(new PlayerFreeMovementState(stateMachine));
            stateMachine.Animator.CrossFadeInFixedTime(MovementHash, 0.1f);
        }
        // Exit if animation has finished
        if (GetAttackNormalizedTime() >= 1f)
        {
            stateMachine.SwitchState(new PlayerFreeMovementState(stateMachine));
            stateMachine.Animator.CrossFadeInFixedTime(MovementHash, 0.1f);
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
}
