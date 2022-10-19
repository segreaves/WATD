using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    private EnemyAIBrain enemyBrain;
    Collider DamageCollider;

    private void Awake()
    {
        enemyBrain = GetComponent<EnemyAIBrain>();
        DamageCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var hittable = other.GetComponent<IHittable>();
            hittable?.GetHit(1, gameObject);
        }
    }

    public void EnableDamage(bool value)
    {
        DamageCollider.enabled = value;
    }
}
