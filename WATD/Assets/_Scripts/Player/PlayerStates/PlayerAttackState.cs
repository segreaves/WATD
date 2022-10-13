using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : State
{
    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    AttackDataSO attackData;

    public override void Enter()
    {
        //stateMachine.SelectAttack();
        //stateMachine.Animator.CrossFadeInFixedTime(stateMachine.attackId.AnimationName, stateMachine.attackId.TransitionDuration);
    }

    public override void Exit() {}

    public override void Tick(float deltaTime) {}
}
