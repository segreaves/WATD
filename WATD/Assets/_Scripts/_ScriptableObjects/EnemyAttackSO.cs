using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Attack")]
public class EnemyAttackSO : ScriptableObject
{
    [SerializeField] public string AttackAnimation;
    [SerializeField] [Range(0f, 25f)] public float RotationSpeed = 25f;
    [SerializeField] [Range(0f, 10f)] public float HitCapsuleForwardOffset = 1f;
    [SerializeField] [Range(0f, 10f)] public float HitCapsuleRadius = 1f;
    [SerializeField] [Range(0f, 10f)] public float HitCapsuleHeight = 1f;
}
