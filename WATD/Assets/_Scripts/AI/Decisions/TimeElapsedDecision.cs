using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeElapsedDecision : AIDecision
{
    [field: SerializeField] [Range(0f, 10f)] public float MinDuration = 1f;
    [field: SerializeField] [Range(0f, 10f)] public float RandomDuration = 1f;
    private float randomValue;

    private void Start()
    {
        ResetRandomValue();
    }
    public override bool MakeDecision()
    {
        bool decision = enemyBrain.CurrentState.aiActionData.TimeElapsed >= randomValue;
        if (decision == true)
        {
            enemyBrain.CurrentState.ResetStateTimer();
            ResetRandomValue();
        }
        return decision;
    }

    private void ResetRandomValue()
    {
        randomValue = Random.Range(MinDuration, MinDuration + RandomDuration);
    }
}
