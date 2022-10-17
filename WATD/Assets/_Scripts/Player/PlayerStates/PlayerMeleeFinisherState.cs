using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeFinisherState : PlayerMeleeBaseState
{
    public PlayerMeleeFinisherState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        // Check state exit
        if (attackTimer > currentWeaponData.MaxDuration)
        {
            // Check for actions to transition into
            if (shouldCombo)
            {
                stateMachine.SwitchState(new PlayerMeleeEntryState(stateMachine));
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
