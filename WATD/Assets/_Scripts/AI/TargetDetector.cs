using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : Detector
{
    [SerializeField] private float targetDetectionRange = 5f;
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private GameObject Target;
    [SerializeField] private bool showGizmos = false;

    Vector3 lookOffset = Vector3.zero;//up * 0.2f;

    //gizmo parameters
    private Transform targetGizmo;

    public override void Detect(AIData aIData)
    {
        if (Target == null)
        {
            aIData.currentTarget = null;
            targetGizmo = null;
        }
        else
        {
            Vector3 direction = (Target.transform.position - transform.position).normalized;
            float distanceToPlayer = (Target.transform.position - transform.position).magnitude;
            RaycastHit hit;
            Vector3 rayOrigin = transform.position + lookOffset;
            bool obstacleHit = Physics.Raycast(rayOrigin, direction, out hit, distanceToPlayer, obstaclesLayerMask);
            if (!obstacleHit && distanceToPlayer < targetDetectionRange)
            {
                Debug.DrawRay(rayOrigin, direction * distanceToPlayer, Color.magenta);
                aIData.currentTarget = Target;
                targetGizmo = aIData.currentTarget.transform;
            }
            else
            {
                aIData.currentTarget = null;
                targetGizmo = null;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos == false) { return; }

        Gizmos.DrawWireSphere(transform.position, targetDetectionRange);

        if (targetGizmo == null) { return; }
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(targetGizmo.position, 0.3f);
    }
}
