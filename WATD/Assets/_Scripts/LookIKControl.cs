using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LookIKControl : MonoBehaviour
{
    [field: SerializeField] private GameObject LookTarget;
    [field: SerializeField] private Rig lookRig;
    private Vector3 dampingVelocity;
    private float baseSolveSpeed = 0.05f;
    private float desiredWeight;

    private void Update()
    {
        lookRig.weight = Mathf.Lerp(lookRig.weight, desiredWeight, Time.deltaTime);
    }
    public void StartLooking()
    {
        desiredWeight = 1f;
    }

    public void StopLooking()
    {
        desiredWeight = 0f;
    }

    public void LookAt(Vector3 lookPosition, float solveSpeed)
    {
        LookTarget.transform.position = Vector3.SmoothDamp(LookTarget.transform.position, lookPosition, ref dampingVelocity, solveSpeed);
    }

    public void LookAt(Vector3 lookPosition)
    {
        LookAt(lookPosition, baseSolveSpeed);
    }
}