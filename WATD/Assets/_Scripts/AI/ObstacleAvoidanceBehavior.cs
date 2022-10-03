using System.Collections;
using UnityEngine;

public class ObstacleAvoidanceBehavior : SteeringBehavior
{
    [SerializeField] private float radius = 2f, agentColliderSize = 0.6f;
    [SerializeField] private bool showGizmo = true;

    //gizmo parameters
    float[] dangersResultTemp = null;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        foreach (Collider obstacleCollider in aiData.obstacles)
        {
            Vector3 directionToObstacle = obstacleCollider.ClosestPoint(transform.position) - transform.position;
            directionToObstacle.y = 0f;
            float distanceToObstacle = directionToObstacle.magnitude;

            //calculate weight based on the distance Enemy<--->Obstacle
            float weight = distanceToObstacle <= agentColliderSize ? 1 : (radius - distanceToObstacle) / radius;

            Vector3 directionToObstacleNormalized = directionToObstacle.normalized;

            //Add obstacle parameters to the danger array
            for (int i = 0; i < Directions.eightDirections.Count; i++)
            {
                float result = Vector3.Dot(directionToObstacleNormalized, Directions.eightDirections[i]);

                float valueToPutIn = result * weight;

                //override value only if it is higher than the current one stored in the danger array
                if (valueToPutIn > danger[i])
                {
                    danger[i] = valueToPutIn;
                }
            }
        }
        dangersResultTemp = danger;
        return (danger, interest);
    }

    private void OnDrawGizmos()
    {
        if (showGizmo == false)
            return;

        if (Application.isPlaying && dangersResultTemp != null)
        {
            if (dangersResultTemp != null)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < dangersResultTemp.Length; i++)
                {
                    Gizmos.DrawRay(
                        transform.position,
                        Directions.eightDirections[i] * dangersResultTemp[i]*2
                        );
                }
            }
        }

    }
}
