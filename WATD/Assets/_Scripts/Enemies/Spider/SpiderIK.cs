using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderIK : MonoBehaviour
{
    private CharacterController Controller;
    public Transform[] legTargets;
    private Vector3[] defaultLegPositions;
    private Vector3[] lastLegPositions;
    private Vector3[] targetPositions;
    private int nbLegs;
    public float stepSize = 0.15f;
    private bool[] legMoving;
    private Vector3 velocity;

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        nbLegs = legTargets.Length;
        defaultLegPositions = new Vector3[nbLegs];
        lastLegPositions = new Vector3[nbLegs];
        targetPositions = new Vector3[nbLegs];
        legMoving = new bool[nbLegs];
        for (int i = 0; i < nbLegs; ++i)
        {
            defaultLegPositions[i] = legTargets[i].transform.position - transform.position;
            lastLegPositions[i] = legTargets[i].position;
            legMoving[i] = false;
        }
    }

    private void Update()
    {
        velocity = Controller.velocity;
        Vector3[] desiredPositions = new Vector3[nbLegs];
        int indexToMove = -1;
        float maxDistance = stepSize;
        for (int i = 0; i < nbLegs; ++i)
        {
            desiredPositions[i] = transform.TransformPoint(defaultLegPositions[i]);

            float distance = Vector3.ProjectOnPlane(desiredPositions[i] - lastLegPositions[i], transform.up).magnitude;
            if (distance > maxDistance)
            {
                maxDistance = distance;
                indexToMove = i;
            }
        }
        for (int i = 0; i < nbLegs; ++i)
        {
            if (i != indexToMove)
            {
                legTargets[i].position = lastLegPositions[i];
            }
            Vector3[] positionAndNormalFwd = MatchToSurfaceFromAbove(defaultLegPositions[i], 1.5f, transform.up);
            targetPositions[i] = positionAndNormalFwd[0];
        }
    }

    Vector3[] MatchToSurfaceFromAbove(Vector3 point, float halfRange, Vector3 up)
    {
        Vector3[] res = new Vector3[2];
        res[1] = Vector3.zero;
        RaycastHit hit;

        if (Physics.Raycast(point + up, -up, out hit, 2f * halfRange))
        {
            res[0] = hit.point;
            res[1] = hit.normal;
        }
        else
        {
            res[0] = point;
        }
        return res;
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < nbLegs; ++i)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(legTargets[i].position, 0.05f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.TransformPoint(defaultLegPositions[i]), stepSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.TransformPoint(targetPositions[i]), 0.2f);
        }
    }
}
