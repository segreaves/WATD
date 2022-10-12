using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerData")]
public class PlayerDataSO : ScriptableObject
{
    [field: SerializeField] [Range(1, 10)] public int MaxHealth = 3;
    [field: SerializeField] [Range(1, 10)] public int Damage = 1;
    [field: SerializeField] [Range(0.1f, 0.5f)] public float DashDuration = 0.25f;
    [field: SerializeField] [Range(0.1f, 0.25f)] public float DashInvulnerability = 0.1f;
    [field: SerializeField] [Range(10f, 30f)] public float DashImpulse = 20f;
    [field: SerializeField] [Range(0.1f, 0.5f)] public float DashCooldown = 0.35f;
}
