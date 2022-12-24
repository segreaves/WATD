using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Melee")]
public class MeleeWeaponSO : ScriptableObject
{
    [field: SerializeField] public string WeaponName;
    [field: SerializeField] public GameObject WeaponPrefab;
    [field: SerializeField] public List<string> AttackAnimations;
    [field: SerializeField] [Range(1, 100)] public int Damage = 1;
    [field: SerializeField] [Range(0.1f, 0.5f)] public float ComboStartTime = 0.1f;
    [field: SerializeField] [Range(0.1f, 2f)] public float AttackDuration = 0.5f;
    [field: SerializeField] [Range(0.1f, 0.5f)] public float Cooldown = 0.2f;
    [field: SerializeField] [Range(0f, 50f)] public float RotationSpeed = 25f;
    [field: SerializeField] [Range(0f, 0.25f)] public float RotationDuration = 0.25f;
    [field: SerializeField] [Range(0f, 10f)] public float HitCapsuleForwardOffset = 1f;
    [field: SerializeField] [Range(0f, 10f)] public float HitCapsuleRadius = 1f;
    [field: SerializeField] [Range(0f, 10f)] public float HitCapsuleHeight = 1f;
}
