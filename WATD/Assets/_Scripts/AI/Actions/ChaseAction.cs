using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseAction : AIAction
{
    public override void Enter() {}

    public override void Exit() {}

    public override void Tick()
    {
        var direction = enemyBrain.Target.transform.position - transform.position;
        aiMovementData.Direction = direction.normalized;
        aiMovementData.PointOfInterest = enemyBrain.Target.transform.position;
        enemyBrain.Move(aiMovementData.Direction);
        enemyBrain.LookAt(aiMovementData.PointOfInterest);
    }
}
