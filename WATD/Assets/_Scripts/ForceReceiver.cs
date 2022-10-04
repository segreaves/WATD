using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class ForceReceiver : MonoBehaviour
{
    private CharacterController controller;
    private NavMeshAgent agent;
    [SerializeField] private float drag = 0.3f;
    private Vector3 impact;
    private Vector3 dampingVelocity;
    private float verticalVelocity;
    public Vector3 Movement => impact + Vector3.up * verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Gravity
        if (verticalVelocity < 0f && controller.isGrounded)
        {
            verticalVelocity = Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
        // Reduce the value of impact towards zero
        impact = Vector3.SmoothDamp(impact, Vector3.zero, ref dampingVelocity, drag);
        // Turn off navmesh while receiving impact so it won't fight it
        if (agent != null && impact.sqrMagnitude <= 0.2f * 0.2f)
        {
            impact = Vector3.zero;
            agent.enabled = true;
        }
    }

    public virtual void AddForce(Vector3 force)
    {
        impact += force;
        if (agent != null)
        {
            agent.enabled = false;
        }
    }

    public void AddFwdForce(float forceValue = 15f)
    {
        Vector3 force = gameObject.transform.forward * forceValue;
        AddForce(force);
    }

    public void ResetImpact()
    {
        // This function stops whatever forces were affecting the object
        impact = Vector3.zero;
    }
}
