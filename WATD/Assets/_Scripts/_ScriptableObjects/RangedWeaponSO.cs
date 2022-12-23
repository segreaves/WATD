using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Ranged/Weapon")]
public class RangedWeaponSO : ScriptableObject
{
    [field: SerializeField] public GameObject projectile;
    [field: SerializeField] [Range(1, 10)] public int Damage;
    [field: SerializeField] public string ShootAnimation;
    [field: SerializeField] public float ShootForce;
    [field: SerializeField] public bool AllowButtonHold;
    [field: SerializeField] [Range(0.1f, 0.5f)] public float TimeBetweenShooting = 0.2f, TimeBetweenShots = 0.2f;
    [field: SerializeField] [Range(0f, 45f)] public float Spread;
    [field: SerializeField] [Range(1, 10)] public int BulletsPerTap;
}
