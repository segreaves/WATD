using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    public Animator animator;
    public LookIKControl LookIKControl;
    public readonly int DashHash = Animator.StringToHash("Dash");
    public Vector3 LookDirection;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        LookIKControl = GetComponent<LookIKControl>();
    }

    private void Start()
    {
        LookDirection = transform.forward;
    }

    public void PlayTargetAnimation(int targetAnimHash, bool isInteracting, float transitionDuration)
    {
        animator.applyRootMotion = isInteracting;
        animator.SetBool("IsInteracting", isInteracting);
        animator.CrossFadeInFixedTime(targetAnimHash, transitionDuration);
    }
}
