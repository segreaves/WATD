using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float maxHealth = 3;
    [SerializeField] private float health;

    private void Start()
    {
        health = maxHealth;
    }

    public void DealDamage(float damage)
    {
        if (health == 0) { return; }
        // Remove damage from health
        health = Mathf.Max(health - damage, 0);
    }

    public bool IsAlive()
    {
        return health > 0f;
    }
}
