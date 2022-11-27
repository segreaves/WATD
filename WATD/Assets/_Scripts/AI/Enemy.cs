using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(ForceReceiver))]
[RequireComponent(typeof(FlashComponent))]
[RequireComponent(typeof(SlowMotionComponent))]
[RequireComponent(typeof(Target))]
public class Enemy : MonoBehaviour, IHittable
{
    [field: SerializeField] public EnemyDataSO EnemyData { get; set; }
    [field: SerializeField] public UnityEvent<bool> OnMeleeEnabled { get; set; }
    [field: SerializeField] public UnityEvent OnGetHit { get; set; }
    [field: SerializeField] public UnityEvent OnDie { get; set; }
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    
    private void Awake()
    {
        Health = transform.root.GetComponent<Health>();
        Health.maxHealth = EnemyData.MaxHealth;
        ForceReceiver = transform.root.GetComponent<ForceReceiver>();
    }

    public void GetHit(int damage, GameObject damageDealer)
    {
        // Process hit
        if (!Health.IsAlive()) { return; }
        Health.DealDamage(damage);
        if (Health.IsAlive())
        {
            OnGetHit?.Invoke();
            CameraShake.Instance.ShakeCamera(1f, 0.1f);
        }
        else
        {
            // Add damage impulse
            Vector3 damageDirection = transform.position - damageDealer.transform.position;
            damageDirection.y = 0f;
            ForceReceiver?.ResetImpact();
            ForceReceiver?.AddForce(damageDirection.normalized * 5f);
            OnDie?.Invoke();
            CameraShake.Instance.ShakeCamera(5f, 0.1f);
        }
    }

    public void MeleeEnabled()
    {
        OnMeleeEnabled?.Invoke(true);
    }

    public void MeleeDisabled()
    {
        OnMeleeEnabled?.Invoke(false);
    }
}
