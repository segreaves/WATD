using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigControl_Aim : MonoBehaviour
{
    private RigBuilder rigBuilder;
    [SerializeField] private GameObject lookRay;
    [SerializeField] private GameObject lookTarget;
    [SerializeField] private GameObject target;
    [SerializeField] float speedChange = 0.1f;
    private PlayerStateMachine stateMachine;
    private Vector3 dampingVelocity;

    private void Awake()
    {
        rigBuilder = GetComponent<RigBuilder>();
        stateMachine = GetComponent<PlayerStateMachine>();
    }

    private void Update()
    {
        if (stateMachine.InputReceiver.lookInput)
        {
            lookRay.transform.rotation = Quaternion.LookRotation(stateMachine.InputReceiver.LookValue);
        }
        else
        {
            lookRay.transform.rotation = Quaternion.LookRotation(gameObject.transform.forward);
        }
        target.transform.position = Vector3.SmoothDamp(target.transform.position, lookTarget.transform.position, ref dampingVelocity, speedChange);
    }

    public void EnableRig()
    {
        rigBuilder.enabled = true;
    }

    public void DisableRig()
    {
        rigBuilder.enabled = false;
    }
}
