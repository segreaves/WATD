using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextSolver : MonoBehaviour
{
    float[] interestCache;
    float[] dangerCache;
    Vector3 direction;

    public Vector3 GetDirectionToMove(List<AIAction> actions)
    {
        float[] danger = new float[Directions.eightDirections.Count];
        float[] interest = new float[Directions.eightDirections.Count];

        // Loop through each behaviour
        foreach (AIAction action in actions)
        {
            (danger, interest) = action.GetSteering(danger, interest);
        }
        dangerCache = danger;

        // Subtract danger values from interest array
        for (int i = 0; i < Directions.eightDirections.Count; i++)
        {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
        }
        interestCache = interest;

        // Get the average direction
        Vector3 outputDirection = Vector3.zero;
        for (int i = 0; i < Directions.eightDirections.Count; i++)
        {
            outputDirection += Directions.eightDirections[i] * interest[i];
        }

        //outputDirection.Normalize();
        direction = outputDirection;

        // Return the selected movement direction
        return outputDirection;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // Interest
            if (interestCache != null)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < interestCache.Length; i++)
                {
                    Gizmos.DrawRay(transform.position, Directions.eightDirections[i] * interestCache[i] * 2);
                }
            }
            // Danger
            if (dangerCache != null)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < dangerCache.Length; i++)
                {
                    Gizmos.DrawRay(transform.position, Directions.eightDirections[i] * dangerCache[i] * 2);
                }
            }
            // Final direction
            if (direction != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, direction * 2.5f);
            }
        }
    }
}
