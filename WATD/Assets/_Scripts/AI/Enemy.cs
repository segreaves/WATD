using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour, IHittable, IAgent
{
    [field: SerializeField] public EnemyDataSO EnemyData { get; set; }
    [field: SerializeField] public UnityEvent OnGetHit { get; set; }
    [field: SerializeField] public UnityEvent OnDie { get; set; }
    [field: SerializeField] public int Health { get; private set; } = 1;
    
    private void Start()
    {
        Health = EnemyData.MaxHealth;
    }

    public void GetHit(int damage, GameObject damageDealer)
    {
        Health--;
        OnGetHit?.Invoke();
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}