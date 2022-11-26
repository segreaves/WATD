using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderIK : MonoBehaviour
{
    private CharacterController Controller;
    public Transform[] legTargets;
    private Vector3[] Targets;
    private Vector3[] defaultLegPositions;
    private Vector3[] desiredLegPositions;
    private Vector3[] rayOriginPositions;
    private Vector3 lastPosition;
    private Vector3[] stepStartPosition;
    private Vector3[] lastLegPositions;
    [SerializeField] private AnimationCurve LegHeightCurve;
    private bool legMoving;
    private int legs;
    private float stepSize = 0.3f;
    public float stepHeight = 0.25f;
    private float raycastRange = 1.5f;
    private float velocityMultiplier = 50f;
    private int indexToMove;
    private float stepTimer;
    private float stepDuration = 0.2f;
    private Vector3 velocity;
     private Vector3 currentVelocity;
    private Vector3 dampingVelocity;
    private float speed;

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        lastPosition = transform.position;
        legs = legTargets.Length;
        defaultLegPositions = new Vector3[legs];
        desiredLegPositions = new Vector3[legs];
        lastLegPositions = new Vector3[legs];
        Targets = new Vector3[legs];
        rayOriginPositions = new Vector3[legs];
        stepStartPosition = new Vector3[legs];
        legMoving = false;
        for (int i = 0; i < legs; ++i)
        {
            defaultLegPositions[i] = legTargets[i].transform.position - transform.position;
            lastLegPositions[i] = legTargets[i].transform.position;
        }
    }

    private void Update()
    {
        velocity = transform.position - lastPosition;
        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref dampingVelocity, 0.025f);
        speed = currentVelocity.magnitude / Time.deltaTime;
        SetLegDesiredPositions();
        SetLegTargetPositions();
        if (legMoving == false)
        {
            SetMovingLeg();
        }
        FixNonMovingLegs();
        if (stepTimer > 0)
        {
            float stepRatio = stepTimer / stepDuration;
            stepTimer -= Time.deltaTime;
            for (int i = 0; i < legs; ++i)
            {
                if (i % 2 == indexToMove % 2)
                {
                    MoveLeg(i, stepRatio);
                }
            }
        }
        else
        {
            if (legMoving == true)
            {
                legMoving = false;
            }
        }
        lastPosition = transform.position;
    }

    private void StartStep(int index)
    {
        float currentStepSize = (Targets[index] - lastLegPositions[index]).magnitude;
        stepDuration = Mathf.Clamp((currentStepSize / speed) / legs, 0.01f, 0.25f);
        stepTimer = stepDuration;
        legMoving = true;
        for (int i = 0; i < legs; ++i)
        {
            stepStartPosition[i] = lastLegPositions[i];
        }
    }
    private void SetLegDesiredPositions()
    {
        for (int i = 0; i < legs; ++i)
        {
            Vector3 basePosition = transform.TransformPoint(defaultLegPositions[i]);
            rayOriginPositions[i] = basePosition + transform.up;
            // Targets
            RaycastHit hit;
            if (Physics.Raycast(rayOriginPositions[i], -transform.up, out hit, raycastRange))
            {
                desiredLegPositions[i] = hit.point;
            }
            else
            {
                desiredLegPositions[i] = basePosition;
            }
        }
    }

    private void SetLegTargetPositions()
    {
        Vector3 correctForVelocity = Vector3.ClampMagnitude(currentVelocity * velocityMultiplier, stepSize);
        for (int i = 0; i < legs; ++i)
        {
            // Targets
            RaycastHit hit;
            if (Physics.Raycast(desiredLegPositions[i] + transform.up + correctForVelocity, -transform.up, out hit, raycastRange))
            {
                Targets[i] = hit.point;
            }
            else
            {
                Targets[i] = lastLegPositions[i];
            }
        }
    }

    private void SetMovingLeg()
    {
        indexToMove = -1;
        float maxDistance = stepSize;
        for (int i = 0; i < legs; ++i)
        {
            // Check if legs are within safe area
            float distance = Vector3.ProjectOnPlane(desiredLegPositions[i] - lastLegPositions[i], transform.up).magnitude;
            // Leg farthest from desired position will be one to move unless distance is under step size
            if (distance > maxDistance)
            {
                maxDistance = distance;
                indexToMove = i;
                StartStep(indexToMove);
            }
        }
    }

    private void FixNonMovingLegs()
    {
        // Fix legs that are in safe area to ground
        for (int i = 0; i < legs; ++i)
        {
            if (i != indexToMove)
            {
                legTargets[i].transform.position = lastLegPositions[i];
            }
        }
    }

    private void MoveLeg(int index, float stepRatio)
    {
        if (legMoving == false) { return; }
        if (index == -1) { return; }
        legTargets[index].transform.position = Vector3.Lerp(Targets[index], stepStartPosition[index], stepRatio);
        legTargets[index].position += transform.up * LegHeightCurve.Evaluate(stepRatio) * stepHeight;
        lastLegPositions[index] = legTargets[index].transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            // Movement direction
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + currentVelocity);
            Gizmos.DrawWireSphere(transform.position + currentVelocity, stepSize);
            for (int i = 0; i < legs; ++i)
            {
                if (indexToMove == i)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawWireSphere(desiredLegPositions[i], stepSize);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(rayOriginPositions[i], Targets[i]);
                Gizmos.DrawWireSphere(Targets[i], 0.1f);
            }
            if (indexToMove != -1 && legMoving == true)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(Targets[indexToMove] + transform.up, Targets[indexToMove]);
            }
        }
    }
}
