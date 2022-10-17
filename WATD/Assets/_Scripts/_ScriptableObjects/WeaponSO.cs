using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon")]
public class WeaponSO : ScriptableObject
{
    [field: SerializeField] public List<string> AttackAnimations;
    [field: SerializeField] [Range(0.1f, 0.5f)] public float TransitionDuration = 0.1f;
    [field: SerializeField] [Range(0.1f, 0.5f)] public float ComboStartTime = 0.1f;
    [field: SerializeField] [Range(0.1f, 2f)] public float AttackDuration = 0.5f;
    [field: SerializeField] [Range(0.1f, 2f)] public float MaxDuration = 0.5f;
    [field: SerializeField] [Range(0f, 25f)] public float RotationSpeed = 25f;
    [field: SerializeField] [Range(0f, 0.25f)] public float RotationDuration = 0.25f;
}
