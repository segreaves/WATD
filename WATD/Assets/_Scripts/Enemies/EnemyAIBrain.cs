using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAIBrain : MonoBehaviour, IAgentInput
{
    [field: SerializeField] public GameObject Target { get; set; }
    [field: SerializeField] public AIState CurrentState { get; private set; }
    [field: SerializeField] public UnityEvent<Vector3> OnMovement { get; set; }
    [field: SerializeField] public UnityEvent<Vector3> OnFaceDirection { get; set; }
    [field: SerializeField] public UnityEvent OnAttack { get; set; }

    private void Awake()
    {
        Target = FindObjectOfType<Player>().gameObject;
    }

    private void Update()
    {
        if (Target == null)
        {
            OnMovement?.Invoke(Vector3.zero);
        }
        else
        {
            CurrentState.UpdateState();
        }
    }

    public void Attack()
    {
        OnAttack?.Invoke();
    }

    public void Move(Vector3 movementDirection, Vector3 targetPosition)
    {
        OnMovement?.Invoke(movementDirection);
        OnFaceDirection?.Invoke(targetPosition);
    }

    internal void SwitchState(AIState state)
    {
        CurrentState = state;
    }
}
