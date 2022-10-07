using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAIBrain : MonoBehaviour, IAgentInput
{
    [field: SerializeField] public AgentMovement MovementParameters { get; set; }
    [field: SerializeField] public GameObject Target { get; set; }
    [field: SerializeField] public AIState CurrentState { get; private set; }
    [field: SerializeField] public ContextSolver MovementDirectionSolver { get; set; }
    [field: SerializeField] public UnityEvent<Vector3> OnMovement { get; set; }
    [field: SerializeField] public UnityEvent<Vector3> OnFaceDirection { get; set; }
    [field: SerializeField] public UnityEvent OnAttack { get; set; }

    private void Awake()
    {
        Target = FindObjectOfType<Player>().gameObject;
        MovementDirectionSolver = transform.root.GetComponent<ContextSolver>();
        MovementParameters = transform.root.GetComponent<AgentMovement>();
    }

    private void Start()
    {
        CurrentState?.Enter();
    }

    private void Update()
    {
        CurrentState?.Tick();
    }

    public void Attack()
    {
        OnAttack?.Invoke();
    }

    public void Move(Vector3 movementDirection)
    {
        OnMovement?.Invoke(movementDirection);
    }

    public void LookAt(Vector3 targetPosition)
    {
        Vector3 lookDirection = targetPosition - transform.position;
        OnFaceDirection?.Invoke(lookDirection.normalized);
    }

    internal void SwitchState(AIState state)
    {
        CurrentState?.Exit();
        CurrentState = state;
        CurrentState?.Enter();
    }
}
