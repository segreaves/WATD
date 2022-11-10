using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Ranged/Projectile")]
public class ProjectileSO : ScriptableObject
{
    [field: SerializeField] [Range(1, 100)] public int Damage = 1;
}
