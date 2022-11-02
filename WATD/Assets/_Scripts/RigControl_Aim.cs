using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigControl_Aim : MonoBehaviour
{
    [SerializeField] private Rig rig;
    [SerializeField] private GameObject target;
    private PlayerStateMachine stateMachine;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        rig.weight = 0f;
    }

    private void Update()
    {
        Vector3 targetDirection;
        if (stateMachine.InputReceiver.lookInput)
        {
            targetDirection = stateMachine.InputReceiver.LookValue;
        }
        else
        {
            targetDirection = gameObject.transform.forward;
        }
        target.transform.position = stateMachine.gameObject.transform.position + targetDirection * 5f;
    }

    public void SetEnabled(bool value)
    {
        rig.weight = value ? 1f : 0f;
    }
}
