using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIAction : MonoBehaviour
{
    protected AIActionData aiActionData;
    protected AIMovementData aiMovementData;
    protected EnemyAIBrain enemyBrain;
    protected CharacterController Controller;
    protected NavMeshAgent Agent;

    private void Awake()
    {
        aiActionData = transform.root.GetComponentInChildren<AIActionData>();
        aiMovementData = transform.root.GetComponentInChildren<AIMovementData>();
        enemyBrain = transform.root.GetComponent<EnemyAIBrain>();

        Controller = transform.root.GetComponent<CharacterController>();
        Agent = transform.root.GetComponent<NavMeshAgent>();
        // We don't want the navmesh agent to move the character for us
        Agent.updatePosition = true;
        Agent.updateRotation = true;
        Agent.enabled = false;
    }

    public abstract void Enter();
    public abstract void Tick();
    public abstract void Exit();

    public virtual (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest)
    {
        return (danger, interest);
    }
}
