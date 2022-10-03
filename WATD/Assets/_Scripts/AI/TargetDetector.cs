using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : Detector
{
    [SerializeField] private float targetDetectionRange = 5f;
    [SerializeField] private LayerMask obstaclesLayerMask;
    [SerializeField] private GameObject Player;
    [SerializeField] private bool showGizmos = false;

    //gizmo parameters
    private List<Transform> colliders;

    public override void Detect(AIData aIData)
    {
        if (Player != null)
        {
            Vector3 direction = (Player.transform.position - transform.position).normalized;
            float distanceToPlayer = (Player.transform.position - transform.position).magnitude;
            RaycastHit hit;
            bool obstacleHit = Physics.Raycast(transform.position, direction, out hit, distanceToPlayer, obstaclesLayerMask);
            if (!obstacleHit && distanceToPlayer < targetDetectionRange)
            {
                Debug.DrawRay(transform.position, direction * distanceToPlayer, Color.magenta);
                colliders = new List<Transform> { Player.transform };
            }
            else
            {
                colliders = null;
            }
        }
        else
        {
            colliders = null;
        }
        aIData.targets = colliders;
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos == false) { return; }

        Gizmos.DrawWireSphere(transform.position, targetDetectionRange);

        if (colliders == null) { return; }
        Gizmos.color = Color.magenta;
        foreach (var item in colliders)
        {
            Gizmos.DrawSphere(item.position, 0.3f);
        }
    }
}
