using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMovementStateBase : State
{
    protected PlayerMovementStateBase(PlayerStateMachine stateMachine) : base(stateMachine) {}

    protected readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    protected readonly int FacingAngleHash = Animator.StringToHash("FacingAngle");
    protected readonly int LookAngleHash = Animator.StringToHash("LookAngle");
    protected readonly int LookOffsetHash = Animator.StringToHash("LookOffset");
    protected readonly int LArmOutHash = Animator.StringToHash("ArmL");
    protected readonly int RArmOutHash = Animator.StringToHash("ArmR");
    protected Vector3 facingDirection;
    //private Vector3 dampVelocity;
    private float lookAngle;

    public override void Enter()
    {
        facingDirection = stateMachine.AgentMovement.lastDirection;
    }

    public override void Exit() {}

    public override void Tick(float deltaTime) {}

    protected void UpdateAnimationData()
    {
        // Facing angle
        float facingAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.AgentMovement.lastDirection);
        float facingAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.AgentMovement.lastDirection) >= 0f ? -1f : 1f;
        if (facingAngleSign <= 0)
        {
            facingAngle = Math.Clamp(facingAngle, 0f, 180f) / 360f;
        }
        else
        {
            facingAngle = (360f - Math.Clamp(facingAngle, 0f, 180f)) / 360f;
        }
        // Update look angle if not moving
        if (stateMachine.InputReceiver.movementInput == false)
        {
            stateMachine.Animator.SetFloat(FacingAngleHash, facingAngle, 0.0f, Time.deltaTime);
        }

        // Look angle
        lookAngle = Vector3.Angle(stateMachine.transform.forward, stateMachine.InputReceiver.LookValue);
        float lookAngleSign = Vector3.Dot(stateMachine.transform.right, stateMachine.InputReceiver.LookValue) >= 0f ? 1f : -1f;
        float updatelookAngle = 0.5f + lookAngleSign * Math.Clamp(lookAngle, 0f, 180f) / 360f;
        stateMachine.Animator.SetFloat(LookAngleHash, updatelookAngle, 0.025f, Time.deltaTime);
        
        // Set weight of torso/head layer
        //stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("ArmR"), 1f - Mathf.Clamp(stateMachine.AgentMovement.CurrentVelocity, 0f, 0.75f));
        // Look offset
        /*lookDelay = Vector3.SmoothDamp(lookDelay, stateMachine.transform.forward, ref dampVelocity, 0.2f);
        if (stateMachine.InputReceiver.movementInput == true)
        {
            stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("Crouch"), 0f);
        }
        else
        {
            float lookOffset = Quaternion.Angle(stateMachine.transform.rotation, Quaternion.LookRotation(lookDelay));
            lookOffset = Math.Clamp(lookOffset, 0f, 180f) / 180f;
            stateMachine.Animator.SetFloat(LookOffsetHash, lookOffset, 0.0f, Time.deltaTime);
            stateMachine.Animator.SetLayerWeight(stateMachine.Animator.GetLayerIndex("Crouch"), lookOffset);
        }*/
        // Is moving
        stateMachine.Animator.SetBool(IsMovingHash, stateMachine.InputReceiver.movementInput);
    }
}
