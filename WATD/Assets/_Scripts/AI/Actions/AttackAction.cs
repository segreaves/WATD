using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : AIAction
{
    public override void Enter()
    {
        aiActionData.Attack = true;
        enemyBrain.Attack();
    }

    public override void Exit()
    {
        aiActionData.Attack = false;
    }

    public override void Tick() {}
}
