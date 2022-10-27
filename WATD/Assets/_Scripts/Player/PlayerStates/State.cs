using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected PlayerStateMachine stateMachine;

    public State(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    private Vector3 inputLookDirection;

    public abstract void Enter();
    public abstract void Tick(float deltaTime);
    public abstract void Exit();

    protected virtual bool IsMovementState()
    {
        return false;
    }

    protected void UpdateFaceDirection()
    {
        if (stateMachine.InputReceiver.lookInput == false && stateMachine.InputReceiver.movementInput == false)
        {
            // No movement or look input
            inputLookDirection = stateMachine.transform.forward;
        }
        else if (stateMachine.InputReceiver.lookInput == false && stateMachine.InputReceiver.movementInput == true)
        {
            // Face movement direction
            Vector3 velocity = stateMachine.InputReceiver.Controller.velocity;
            velocity.y = 0f;
            if (velocity.sqrMagnitude > 0.01f)
            {
                inputLookDirection = velocity.normalized;
            }
        }
        else if (stateMachine.InputReceiver.lookInput == true && stateMachine.InputReceiver.movementInput == true)
        {
            // Face look direction
            inputLookDirection = stateMachine.InputReceiver.LookValue;
        }
        else
        {
            // Face look direction if > 90 degrees from forward
            float angle = Quaternion.Angle(stateMachine.transform.rotation, Quaternion.LookRotation(stateMachine.InputReceiver.LookValue));
            bool right = Vector3.Dot(stateMachine.transform.right, stateMachine.InputReceiver.LookValue) > 0 ? true : false;
            if (angle >= 90f && 1==0)
            {
                inputLookDirection = stateMachine.InputReceiver.LookValue;
                if (right)
                {
                    TurnR();
                }
                else
                {
                    TurnL();
                }
            }
        }
        stateMachine.InputReceiver.OnFaceDirection?.Invoke(inputLookDirection);
    }

    protected virtual void TurnL()
    {
        return;
    }

    protected virtual void TurnR()
    {
        return;
    }
}
