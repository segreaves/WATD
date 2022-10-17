using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeComboState : PlayerMeleeBaseState
{
    public PlayerMeleeComboState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        // Check state exit
        if (attackTimer > currentWeaponData.AttackDuration)
        {
            // Check for actions to transition into
            if (shouldCombo)
            {
                stateMachine.SwitchState(new PlayerMeleeFinisherState(stateMachine));
            }
            else if (shouldDash)
            {
                stateMachine.SwitchState(new PlayerDashState(stateMachine));
            }
        }
        // Check if should exit attacking entirely
        ExitConditions();
    }
}
