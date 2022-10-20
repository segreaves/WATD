using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : MonoBehaviour
{
    private EnemyAIBrain enemyBrain = null;
    [SerializeField] private List<AIAction> actions = null;
    [SerializeField] private List<AITransition> transitions = null;
    public AIActionData aiActionData { get; private set; }
    protected readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    protected AIMovementData aiMovementData;
    private float rayLength = 3f;

    private void Awake()
    {
        enemyBrain = transform.root.GetComponent<EnemyAIBrain>();
        aiMovementData = transform.root.GetComponentInChildren<AIMovementData>();
        aiActionData = transform.root.GetComponentInChildren<AIActionData>();
    }

    public void Enter()
    {
        ResetStateTimer();
        enemyBrain.Animator.CrossFadeInFixedTime(LocomotionHash, 0.1f);
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
        aiActionData.TimeElapsed += Time.deltaTime;
        // Perform movement
        Vector3 desiredDirection = enemyBrain.MovementDirectionSolver.GetDirectionToMove(actions);
        // Prevent extremely small desiredDirection magnitudes
        if (desiredDirection.sqrMagnitude < 0.005f)
        {
            desiredDirection = Vector3.zero;
        }
        aiMovementData.Direction = Vector3.Lerp(aiMovementData.Direction, desiredDirection, Time.deltaTime * enemyBrain.MovementParameters.MovementData.momentum);
        enemyBrain.Move(aiMovementData.Direction);
        // Perform actions
        foreach (var action in actions)
        {
            action.Tick();
        }
        // Check decisions for transitions.
        // unanimous == true: If any decision is false then break and transition
        // unanimous == false: If any decision is true then break and transition
        foreach (var transition in transitions)
        {
            bool result = false;
            foreach (var decision in transition.Decisions)
            {
                result = decision.MakeDecision();
                if (transition.unanimous)
                {
                    if (result == false) { break; }
                }
                else
                {
                    if (result == true) { break; }
                }
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

    public void ResetStateTimer()
    {
        aiActionData.TimeElapsed = 0f;
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
