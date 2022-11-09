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
}
