using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : AIAction
{
    [SerializeField] List<EnemyAttackSO> AttackData;
    private string animationName;
    private float rotationSpeed;

    public override void Enter()
    {
        aiActionData.Attack = true;
        RandomAttack();
    }

    public override void Exit()
    {
        aiActionData.Attack = false;
    }

    public override void Tick()
    {
        aiActionData.TimeElapsed = GetAttackNormalizedTime();
        if (enemyBrain.TrackTarget)
        {
            // Track target
            aiMovementData.PointOfInterest = enemyBrain.Target.transform.position;
            enemyBrain.RotateTowards(aiMovementData.PointOfInterest, rotationSpeed);
        }
    }

    public void RandomAttack()
    {
        MeleeAttack(Random.Range(0, AttackData.Count));
    }

    public void MeleeAttack(int attackIndex)
    {
        animationName = AttackData[attackIndex].AttackAnimation;
        rotationSpeed = AttackData[attackIndex].RotationSpeed;
        enemyBrain.Animator.CrossFadeInFixedTime(animationName, 0.1f);
    }

    protected float GetAttackNormalizedTime()
    {
        AnimatorStateInfo currentInfo = enemyBrain.Animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextInfo = enemyBrain.Animator.GetNextAnimatorStateInfo(0);
        if (enemyBrain.Animator.IsInTransition(0) && nextInfo.IsTag("Attack"))
        {
            return nextInfo.normalizedTime;
        }
        else if (!enemyBrain.Animator.IsInTransition(0) && currentInfo.IsTag("Attack"))
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f;
        }
    }
}
