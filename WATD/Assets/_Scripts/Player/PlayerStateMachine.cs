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
    [field: SerializeField] public MeleeWeaponHandler MeleeWeaponHandler;
    [field: SerializeField] public RangedWeaponHandler RangedWeaponHandler;
    [field: SerializeField] public LookIKControl LookIKControl;
    public LockIkSwitcher LockIkSwitcher;
    
    public ForceReceiver ForceReceiver { get; private set; }
    public AgentMovement AgentMovement { get; private set; }
    public Animator Animator { get; private set; }

    private void Awake()
    {
        ForceReceiver = GetComponent<ForceReceiver>();
        AgentMovement = GetComponent<AgentMovement>();
        Animator = GetComponent<Animator>();
        MeleeWeaponHandler = GetComponent<MeleeWeaponHandler>();
        RangedWeaponHandler = GetComponent<RangedWeaponHandler>();
        LookIKControl = GetComponent<LookIKControl>();
        LockIkSwitcher = GetComponent<LockIkSwitcher>();
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

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, AgentMovement.lastDirection * 2f);
        }
    }
}
