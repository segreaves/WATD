using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCompletionDecision : AIDecision
{
    [field: SerializeField] [field: Range(0f, 1f)] public float MaxDuration = 1f;

    public override bool MakeDecision()
    {
        return aiActionData.ActionCompletion < 1f;
    }
}
