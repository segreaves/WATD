using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Animator Animator;
    private CharacterController Controller;
    private Collider[] allColliders;
    private Rigidbody[] allRigidbodies;

    private void Awake()
    {
        allColliders = GetComponentsInChildren<Collider>(true);
        allRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        Animator = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();
    }

    public void Start()
    {
        ToggleRagdoll(false);
    }

    public void ToggleRagdoll(bool isRagdoll)
    {
        // Set colliders
        foreach (Collider collider in allColliders)
        {
            if (collider.gameObject.CompareTag("Ragdoll"))
            {
                collider.enabled = isRagdoll;
            }
        }
        // Set kinematic
        foreach (Rigidbody rigidBody in allRigidbodies)
        {
            if (rigidBody.gameObject.CompareTag("Ragdoll"))
            {
                rigidBody.isKinematic = !isRagdoll;
                rigidBody.useGravity = isRagdoll;
            }
        }
        if (isRagdoll)
        {
            Controller.gameObject.layer = LayerMask.NameToLayer("IgnoreAll");
        }
        Animator.enabled = !isRagdoll;
    }
}
