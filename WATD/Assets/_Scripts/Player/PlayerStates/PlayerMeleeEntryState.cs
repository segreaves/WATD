using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeEntryState : PlayerMeleeBaseState
{
    public PlayerMeleeEntryState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        // Check state exit
        if (attackTimer > currentWeaponData.AttackDuration)
        {
            // Check for actions to transition into
            if (shouldCombo)
            {
                stateMachine.SwitchState(new PlayerMeleeComboState(stateMachine));
            }
            else if (shouldDash)
            {
                stateMachine.SwitchState(new PlayerDashState(stateMachine));
            }
        }
        // Start listening for events
        if (attackTimer > currentWeaponData.ComboStartTime && !isListeningForEvents)
        {
            StartListeningForEvents();
        }
    }
}
