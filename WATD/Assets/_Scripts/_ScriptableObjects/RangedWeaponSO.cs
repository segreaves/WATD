using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Ranged/Weapon")]
public class RangedWeaponSO : ScriptableObject
{
    [field: SerializeField] public string WeaponName;
    [field: SerializeField] public GameObject WeaponPrefab;
    [field: SerializeField] public GameObject bullet;
    [field: SerializeField] [Range(1, 10)] public int Damage = 1;
    [field: SerializeField] public string ShootAnimation;
    [field: SerializeField] public float ShootForce = 300f;
    [field: SerializeField] public bool AllowButtonHold;
    [field: SerializeField] [Range(0.1f, 0.5f)] public float TimeBetweenShooting = 0.2f, TimeBetweenShots = 0.2f;
    [field: SerializeField] [Range(0f, 45f)] public float Spread = 3f;
    [field: SerializeField] [Range(1, 10)] public int BulletsPerTap = 1;
    [field: SerializeField] public GameObject MuzzleFlash;
}
