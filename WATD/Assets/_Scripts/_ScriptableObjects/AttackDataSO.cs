using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/AttackData")]
public class AttackDataSO : ScriptableObject
{
    [field: SerializeField] public List<string> AnimationNames;
    [field: SerializeField] [Range(0.1f, 0.5f)] public float TransitionDuration = 0.1f;
    [field: SerializeField] public int NextIndex = -1;
    [field: SerializeField] [Range(0.1f, 0.5f)] public float ComboStartTime = 0.1f;
    [field: SerializeField] [Range(0f, 25f)] public float RotationSpeed = 25f;
}
