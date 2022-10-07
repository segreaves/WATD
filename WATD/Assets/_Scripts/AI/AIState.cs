using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : MonoBehaviour
{
    private EnemyAIBrain enemyBrain = null;
    [SerializeField] private List<AIAction> actions = null;
    [SerializeField] private List<AITransition> transitions = null;
    protected AIMovementData aiMovementData;
    private float rayLength = 3f;

    private void Awake()
    {
        enemyBrain = transform.root.GetComponent<EnemyAIBrain>();
        aiMovementData = transform.root.GetComponentInChildren<AIMovementData>();
    }

    public void Enter()
    {
        foreach (var action in actions)
        {
            action.Enter();
        }
    }

    public void Exit()
    {
        foreach (var action in actions)
        {
            action.Exit();
        }
    }

    public void Tick()
    {
        // Perform movement
        Vector3 desiredDirection = enemyBrain.MovementDirectionSolver.GetDirectionToMove(actions);
        aiMovementData.Direction = Vector3.Lerp(aiMovementData.Direction, desiredDirection, Time.deltaTime * 2f);
        enemyBrain.Move(aiMovementData.Direction);
        // Perform actions
        foreach (var action in actions)
        {
            action.Tick();
        }
        foreach (var transition in transitions)
        {
            bool result = false;
            foreach (var decision in transition.Decisions)
            {
                result = decision.MakeDecision();
                if (result == false) { break; }
            }
            if (result)
            {
                if (transition.PositiveResult != null)
                {
                    enemyBrain.SwitchState(transition.PositiveResult);
                    return;
                }
            }
            else
            {
                if (transition.NegativeResult != null)
                {
                    enemyBrain.SwitchState(transition.NegativeResult);
                    return;
                }
            }
        }
    }

    public List<AIAction> GetActions()
    {
        return actions;
    }

    public List<AITransition> GetTransitiona()
    {
        return transitions;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, aiMovementData.Direction * rayLength);
        }
    }
}
