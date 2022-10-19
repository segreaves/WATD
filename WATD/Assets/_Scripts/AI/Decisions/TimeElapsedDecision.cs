using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeElapsedDecision : AIDecision
{
    [field: SerializeField] [field: Range(0f, 1f)] public float MaxDuration = 1f;

    public override bool MakeDecision()
    {
        return aiActionData.TimeElapsed < 1f;
    }
}
