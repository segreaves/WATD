using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractAction : AIAction
{
    [SerializeField] private float radius;

    public override void Enter() {}

    public override void Exit() {}

    public override void Tick()
    {
        // Look
        aiMovementData.PointOfInterest = enemyBrain.Target.transform.position;
        enemyBrain.FaceDirection(aiMovementData.PointOfInterest);
    }

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest)
    {
        if (enemyBrain.Target == null)
        {
            return (danger, interest);
        }
        else
        {
            var distance = (enemyBrain.Target.transform.position - transform.position).magnitude;
            // Straight line to target
            Vector3 directionToTarget = enemyBrain.Target.transform.position - transform.position;
            float distanceToObstacle = directionToTarget.magnitude;
            directionToTarget.Normalize();
            directionToTarget.y = 0f;
            // Calculate weight based on the distance from enemy to object
            float weight = distanceToObstacle <= radius ? 0 : 1 - Mathf.Clamp01((2 * radius - distanceToObstacle) / radius);
            for (int i = 0; i < interest.Length; i++)
            {
                float result = Vector3.Dot(directionToTarget, Directions.eightDirections[i]);

                // Accept only directions towards target
                if (result > 0)
                {
                    float valueToPutIn = result * weight;
                    if (valueToPutIn > interest[i])
                    {
                        interest[i] = valueToPutIn;
                    }

                }
            }
            return (danger, interest);
        }
    }
}
