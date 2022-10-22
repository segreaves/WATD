using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Ranged")]
public class ProjectileSO : ScriptableObject
{
    [field: SerializeField] string ShootAnimation;
    [field: SerializeField] [Range(1, 100)] public int Damage = 1;
    [field: SerializeField] [Range(0.1f, 0.5f)] public float Cooldown = 0.1f;
}
