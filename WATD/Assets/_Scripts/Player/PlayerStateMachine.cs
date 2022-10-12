using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStateMachine : StateMachine, IAgent
{
    [field: SerializeField] public PlayerInput InputReceiver { get; set; }
    [field: SerializeField] public PlayerDataSO PlayerData { get; set; }
    [field: SerializeField] public int Health { get; set; }
    [field: SerializeField] public UnityEvent OnDie { get; set; }
    [field: SerializeField] public UnityEvent OnGetHit { get; set; }
    
    public ForceReceiver ForceReceiver { get; private set; }
    public AgentMovement AgentMovement { get; private set; }

    private void Awake()
    {
        ForceReceiver = GetComponent<ForceReceiver>();
        AgentMovement = GetComponent<AgentMovement>();
    }

    private void Start()
    {
        Health = PlayerData.MaxHealth;
        SwitchState(new PlayerFreeMovementState(this));
    }
}
