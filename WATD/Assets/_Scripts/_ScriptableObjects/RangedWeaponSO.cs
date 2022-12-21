using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Ranged/Weapon")]
public class RangedWeaponSO : ScriptableObject
{
    [field: SerializeField] public Projectile projectile;
    [field: SerializeField] public string ShootAnimation;
    [field: SerializeField] [Range(0.1f, 0.5f)] public float TimeBetweenShots = 0.2f;
}
