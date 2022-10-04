using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextSolver : MonoBehaviour
{
    [SerializeField] private bool showGizmos = true;

    //gozmo parameters
    float[] interestGizmo = new float[0];
    Vector3 resultDirection = Vector3.zero;
    private float rayLength = 2f;

    private void Start()
    {
        interestGizmo = new float[8];
    }

    public Vector3 GetDirectionToMove(List<SteeringBehavior> behaviors, AIData aiData)
    {
        float[] danger = new float[8];
        float[] interest = new float[8];

        //Loop through each behavior
        foreach (SteeringBehavior behavior in behaviors)
        {
            (danger, interest) = behavior.GetSteering(danger, interest, aiData);
        }

        //subtract danger values from interest array
        for (int i = 0; i < 8; i++)
        {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
        }

        interestGizmo = interest;

        //get the average direction
        Vector3 outputDirection = Vector3.zero;
        for (int i = 0; i < 8; i++)
        {
            outputDirection += Directions.eightDirections[i] * interest[i];
        }

        outputDirection.y = 0f;
        outputDirection.Normalize();

        resultDirection = outputDirection;

        //return the selected movement direction
        return resultDirection;
    }


    private void OnDrawGizmos()
    {
        if (Application.isPlaying && showGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, resultDirection * rayLength);
        }
    }
}
