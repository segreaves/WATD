using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class EnemyAIBrain : MonoBehaviour, IAgentInput
{
    [field: SerializeField] public AgentMovement MovementParameters { get; set; }
    [field: SerializeField] public GameObject Target { get; set; }
    [field: SerializeField] public AIState CurrentState { get; private set; }
    [field: SerializeField] public ContextSolver MovementDirectionSolver { get; set; }
    [field: SerializeField] public UnityEvent<Vector3> OnMovement { get; set; }
    [field: SerializeField] public UnityEvent<Vector3> OnFaceDirection { get; set; }
    [field: SerializeField] public UnityEvent OnAttack { get; set; }
    public NavMeshAgent Agent { get; private set; }
    public CharacterController Controller { get; private set; }

    private void Awake()
    {
        Target = FindObjectOfType<PlayerStateMachine>().gameObject;
        MovementDirectionSolver = transform.root.GetComponent<ContextSolver>();
        MovementParameters = transform.root.GetComponent<AgentMovement>();
        // Controller
        Controller = transform.root.GetComponent<CharacterController>();
        // NavMesh Agent
        Agent = transform.root.GetComponent<UnityEngine.AI.NavMeshAgent>();
        // We don't want the navmesh agent to move the character for us
        Agent.updatePosition = true;
        Agent.updateRotation = false;
    }

    private void Start()
    {
        CurrentState?.Enter();
    }

    private void Update()
    {
        Agent.transform.position = transform.position;
        Agent.velocity = Controller.velocity;
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

    public void FaceDirection(Vector3 targetPosition)
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
