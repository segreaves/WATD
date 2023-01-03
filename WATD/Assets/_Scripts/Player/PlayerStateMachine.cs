using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Health))]
public class PlayerStateMachine : StateMachine, IHittable
{
    [field: SerializeField] public PlayerInput InputHandler { get; set; }
    [field: SerializeField] public PlayerDataSO PlayerData { get; set; }
    [field: SerializeField] public Health Health { get; set; }
    [field: SerializeField] public Power Power { get; set; }
    [field: SerializeField] public UnityEvent OnGetHit { get; set; }
    [field: SerializeField] public UnityEvent OnDie { get; set; }
    [field: SerializeField] public MeleeWeaponHandler MeleeWeaponHandler;
    [field: SerializeField] public RangedWeaponHandler RangedWeaponHandler;
    public AnimatorHandler AnimatorHandler { get; private set; }
    public ForceReceiver ForceReceiver { get; private set; }
    public AgentMovement AgentMovement { get; private set; }
    public Animator Animator { get; private set; }
    public bool IsInteracting { get; private set; }

    private void Awake()
    {
        InputHandler = GetComponent<PlayerInput>();
        ForceReceiver = GetComponent<ForceReceiver>();
        AgentMovement = GetComponent<AgentMovement>();
        Animator = GetComponent<Animator>();
        MeleeWeaponHandler = GetComponent<MeleeWeaponHandler>();
        RangedWeaponHandler = GetComponent<RangedWeaponHandler>();
        AnimatorHandler = GetComponent<AnimatorHandler>();
        Health = GetComponent<Health>();
        Power = GetComponent<Power>();
    }

    private void Start()
    {
        SwitchState(new PlayerFreeMovementState(this));
    }

    protected override void Update()
    {
        base.Update();
        InputHandler.IsInteracting = AnimatorHandler.animator.GetBool(AnimatorHandler.IsInteractingHash);
    }

    public void GetHit(int damage, Vector3 damageDirection)
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
        //Vector3 damageDirection = transform.position - damageDealer.transform.position;
        //damageDirection.y = 0f;
        ForceReceiver?.AddForce(damageDirection.normalized * 5f);
    }
}
