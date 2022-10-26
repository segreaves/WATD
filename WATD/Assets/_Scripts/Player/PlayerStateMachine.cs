using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Health))]
public class PlayerStateMachine : StateMachine, IHittable
{
    [field: SerializeField] public PlayerInput InputReceiver { get; set; }
    [field: SerializeField] public PlayerDataSO PlayerData { get; set; }
    [field: SerializeField] public Health Health { get; set; }
    [field: SerializeField] public UnityEvent OnGetHit { get; set; }
    [field: SerializeField] public UnityEvent OnDie { get; set; }
    [field: SerializeField] public WeaponHandler WeaponHandler;
    
    public ForceReceiver ForceReceiver { get; private set; }
    public AgentMovement AgentMovement { get; private set; }
    public Animator Animator { get; private set; }
    public RigControl_Aim RigControl_Aim { get; private set; }
    public bool isMovementState;

    private void Awake()
    {
        ForceReceiver = GetComponent<ForceReceiver>();
        AgentMovement = GetComponent<AgentMovement>();
        Animator = GetComponent<Animator>();
        WeaponHandler = GetComponent<WeaponHandler>();
        RigControl_Aim = GetComponent<RigControl_Aim>();
        Health = GetComponent<Health>();
        Health.maxHealth = PlayerData.MaxHealth;
    }

    private void Start()
    {
        SwitchState(new PlayerFreeMovementState(this));
    }

    public void GetHit(int damage, GameObject damageDealer)
    {
        // Process hit
        if (!Health.IsAlive()) { return; }
        Health.DealDamage(damage);
        if (Health.IsAlive())
        {
            OnGetHit?.Invoke();
            CameraShake.Instance.ShakeCamera(5f, 0.1f);
        }
        else
        {
            OnDie?.Invoke();
            CameraShake.Instance.ShakeCamera(5f, 0.1f);
        }
        // Add damage impulse
        Vector3 damageDirection = transform.position - damageDealer.transform.position;
        damageDirection.y = 0f;
        ForceReceiver?.AddForce(damageDirection.normalized * 5f);
    }
}
