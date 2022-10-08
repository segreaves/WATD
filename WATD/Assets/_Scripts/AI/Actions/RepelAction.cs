using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepelAction : AIAction
{
    [SerializeField] Targeter detector;
    [SerializeField] private float radius;

    public override void Enter() {}

    public override void Exit() {}

    public override void Tick() {}

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest)
    {
        foreach (Target target in detector.targets)
        {
            Collider obstacleCollider = target.gameObject.GetComponent<Collider>();
            if (obstacleCollider != null)
            {
                Vector3 directionFromObstacle = transform.position - obstacleCollider.ClosestPoint(transform.position);
                directionFromObstacle.y = 0f;
                float distanceToObstacle = directionFromObstacle.magnitude;
                // Calculate weight based on the distance from enemy to object
                float weight = distanceToObstacle > radius ? 0 : Mathf.Clamp01((radius - distanceToObstacle) / radius);

                Vector3 directionToObstacleNormalized = directionFromObstacle.normalized;

                // Add obstacle parameters to the interest array
                for (int i = 0; i < Directions.eightDirections.Count; i++)
                {
                    float result = Vector3.Dot(directionToObstacleNormalized, Directions.eightDirections[i]);

                    float valueToPutIn = result * weight;

                    // Override value only if it is higher than the current one stored in the interest array
                    if (valueToPutIn > interest[i])
                    {
                        interest[i] = valueToPutIn;
                    }
                }
            }
        }
        return (danger, interest);
    }
}
