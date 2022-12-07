using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeMovementState_OLD : PlayerMovementStateBase
{
    public PlayerFreeMovementState_OLD(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        // Right arm layer
        stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("ArmR"), 1f);
        stateMachine.Animator.SetBool(LArmOutHash, false);
        stateMachine.Animator.SetBool(RArmOutHash, false);
        stateMachine.InputHandler.AttackEvent += OnAttack;
        stateMachine.InputHandler.DashEvent += OnDash;
        stateMachine.InputHandler.MeleeEvent += OnMelee;
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.InputHandler.AttackEvent -= OnAttack;
        stateMachine.InputHandler.DashEvent -= OnDash;
        stateMachine.InputHandler.MeleeEvent -= OnMelee;
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        stateMachine.InputHandler.OnMovement?.Invoke(stateMachine.InputHandler.MovementValue);
        //stateMachine.InputHandler.OnWalk.Invoke(stateMachine.InputHandler.lookInput);
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
        if (stateMachine.InputHandler.MovementValue.sqrMagnitude > 0)
        {
            stateMachine.SwitchState(new PlayerDashState(stateMachine));
        }
    }

    private void OnAttack()
    {
        stateMachine.SwitchState(new PlayerMeleeEntryState(stateMachine));
    }
}
