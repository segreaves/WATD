using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeekBehavior : SteeringBehavior
{
    [SerializeField] private float targetReachedThreshold = 1f;
    [SerializeField] private bool showGizmo = true;

    bool reachedTarget = false;

    //gizmo parameters
    private Vector3 targetPositionCached;
    private float[] interestsTemp;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        if (aiData.currentTarget == null)
        {
            return (danger, interest);
        }

        //cache the last position
        targetPositionCached = aiData.currentTarget.transform.position;
        
        //First check if we have reached the target
        float targetDistance = Vector3.Distance(transform.position, targetPositionCached);
        if (targetDistance < targetReachedThreshold)
        {
            reachedTarget = true;
            return (danger, interest);
        }

        //If we haven't yet reached the target then do the main logic of finding the interest directions
        Vector3 directionToTarget = (targetPositionCached - transform.position);
        directionToTarget.y = 0f;
        for (int i = 0; i < interest.Length; i++)
        {
            float result = Vector3.Dot(directionToTarget.normalized, Directions.eightDirections[i]);

            //accept only directions at less than 90 degrees to the target direction
            if (result > 0f)
            {
                float valueToPutIn = result;
                if (valueToPutIn > interest[i])
                {
                    interest[i] = valueToPutIn;
                }

            }
        }
        interestsTemp = interest;
        return (danger, interest);
    }

    private void OnDrawGizmos()
    {

        if (showGizmo == false) { return; }

        Gizmos.DrawSphere(targetPositionCached, 0.2f);

        if (Application.isPlaying && interestsTemp != null)
        {
            if (interestsTemp != null)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < interestsTemp.Length; i++)
                {
                    Gizmos.DrawRay(transform.position, Directions.eightDirections[i] * interestsTemp[i] * 2f);
                }
                if (reachedTarget == false)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(targetPositionCached, 0.1f);
                }
            }
        }
    }
}
