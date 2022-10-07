using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidAction : AIAction
{
    [SerializeField] Targeter detector;

    private float radius;

    private void Start()
    {
        radius = detector.TargetCollider.radius;
    }

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
                Vector3 directionToObstacle = obstacleCollider.ClosestPoint(transform.position) - transform.position;
                directionToObstacle.y = 0f;
                float distanceToObstacle = directionToObstacle.magnitude;
                // Calculate weight based on the distance Enemy<--->Obstacle
                float weight = Mathf.Clamp01((radius - distanceToObstacle) / radius);

                Vector3 directionToObstacleNormalized = directionToObstacle.normalized;

                // Add obstacle parameters to the danger array
                for (int i = 0; i < Directions.eightDirections.Count; i++)
                {
                    float result = Vector3.Dot(directionToObstacleNormalized, Directions.eightDirections[i]);

                    float valueToPutIn = result * weight;

                    // Override value only if it is higher than the current one stored in the danger array
                    if (valueToPutIn > danger[i])
                    {
                        danger[i] = valueToPutIn;
                    }
                }
            }
        }
        return (danger, interest);
    }
}
