using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    public Animator animator;
    public LookIKControl LookIKControl;
    public readonly int IsInteractingHash = Animator.StringToHash("IsInteracting");
    public readonly int FullBodyHash = Animator.StringToHash("Full Body");
    public readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    public readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    public readonly int FacingAngleHash = Animator.StringToHash("FacingAngle");
    public readonly int LookAngleHash = Animator.StringToHash("LookAngle");
    public readonly int DashFHash = Animator.StringToHash("Dash_F");
    public readonly int DashBHash = Animator.StringToHash("Dash_B");
    public Vector3 LastLookDirection;
    public Vector3 LastBodyDirection;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        LookIKControl = GetComponent<LookIKControl>();
    }

    private void Start()
    {
        LastLookDirection = transform.forward;
        LastBodyDirection = transform.forward;
    }

    public void PlayTargetAnimation(int targetAnimHash, bool isInteracting, float transitionDuration)
    {
        animator.SetBool(IsInteractingHash, isInteracting);
        animator.CrossFadeInFixedTime(targetAnimHash, transitionDuration);
    }

    public void PlayTargetAnimation(string targetAnimString, bool isInteracting, float transitionDuration)
    {
        animator.SetBool(IsInteractingHash, isInteracting);
        animator.CrossFadeInFixedTime(targetAnimString, transitionDuration);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, LastBodyDirection * 2f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, LastLookDirection * 2f);
        }
    }
}
