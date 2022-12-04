using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockIkSwitcher : MonoBehaviour
{
    [SerializeField, Tooltip("Please activate the animator IK pass on a specific layer - typically it is most likely best to activate it for the base layer. This is an requirement for getting OnAnimatorIK called on this script (otherwise the script is not going to work). This variable defines the index of the specific layer where the IK pass is activated and will be used by the script.")]
    protected int m_animatorIkPassLayerIndex = 0; public int animatorIkPassLayerIndex { get { return m_animatorIkPassLayerIndex; } set { m_animatorIkPassLayerIndex = value; } }
    [SerializeField] protected bool m_lockLeftFoot; public bool lockLeftFoot { get { return m_lockLeftFoot; } set { m_lockLeftFoot = value; } }
    [SerializeField] protected bool m_lockRightFoot; public bool lockRightFoot { get { return m_lockRightFoot; } set { m_lockRightFoot = value; } }
    [SerializeField] protected bool m_lockRotationsAsWellWhenLocking = true; public bool lockRotationsAsWellWhenLocking { get { return m_lockRotationsAsWellWhenLocking; } set { m_lockRotationsAsWellWhenLocking = value; } }
    [SerializeField, Range(0,10f)] protected float m_transitionSmoothness = 2; public float transitionSmoothness { get { return m_transitionSmoothness; } set { m_transitionSmoothness = value; } }
    [SerializeField, Range(0.01f,10f)] protected float m_maxIkDifferenceDistance = 2; public float maxIkDifferenceDistance { get { return m_maxIkDifferenceDistance; } set { m_maxIkDifferenceDistance = value; } }
    [SerializeField, Range(0,1f)] protected float m_ikWeight = 1.0f; public float ikWeight { get { return m_ikWeight; } set { m_ikWeight = value; } }
    [SerializeField, Range(0,05f)] protected float lockFeetDuration = 0.2f;
    [SerializeField] protected AnimationCurve FootHeightCurve;
    [SerializeField] protected float stepHeight = 0.1f;
    Animator m_animator;

    Vector3 m_leftFootLockedPos;
    Vector3 m_rightFootLockedPos;
    Vector3 m_leftFootTargetPos;
    Vector3 m_rightFootTargetPos;

    bool m_prevLeftLock;
    bool m_prevRightLock;

    float m_currLeftLerp = 0;
    float m_currLeftLerpEndTime = 0;
    float m_currLeftLerpDuration = 0;
    Vector3 m_currLeftStartPos;

    float m_currRightLerp = 0;
    float m_currRightLerpEndTime = 0;
    float m_currRightLerpDuration = 0;
    Vector3 m_currRightStartPos;

    Transform m_leftUpperLeg;
    Transform m_rightUpperLeg;

    Quaternion m_leftFootLockedRot;
    Quaternion m_rightFootLockedRot;
    Quaternion m_leftFootTargetRot;
    Quaternion m_rightFootTargetRot;
    Quaternion m_currLeftStartRot;
    Quaternion m_currRightStartRot;
    private IEnumerator lCoroutine;
    private IEnumerator rCoroutine;

    private void Awake()
    {
        m_animator = this.GetComponent<Animator>();
        m_leftUpperLeg = m_animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        m_rightUpperLeg = m_animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        UpdateFeet();
    }

    public void UpdateFeet()
    {
        Transform LeftFootTransform = m_animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        m_leftFootLockedPos = LeftFootTransform.position;
        m_leftFootTargetPos = m_leftFootLockedPos;
        m_leftFootLockedRot = LeftFootTransform.rotation;
        Transform RightFootTransform = m_animator.GetBoneTransform(HumanBodyBones.RightFoot);
        m_rightFootLockedPos = RightFootTransform.position;
        m_rightFootTargetPos = m_rightFootLockedPos;
        m_rightFootLockedRot = RightFootTransform.rotation;
    }

    public void LockFeet()
    {
        lCoroutine = ELockLFoot(lockFeetDuration);
        StartCoroutine(lCoroutine);
        rCoroutine = ELockRFoot(lockFeetDuration);
        StartCoroutine(rCoroutine);
    }

    public void UnlockFeet()
    {
        if (lockLeftFoot == true)
        {
            StopCoroutine(lCoroutine);
            lockLeftFoot = false;
        }
        if (lockRightFoot == true)
        {
            StopCoroutine(rCoroutine);
            lockRightFoot = false;
        }
    }

    private IEnumerator EUnlockFeet()
    {
        StopCoroutine(lCoroutine);
        yield return new WaitForSeconds(lockFeetDuration / 2f);
        StopCoroutine(rCoroutine);
    }

    private IEnumerator ELockLFoot(float waitTime)
    {
        lockLeftFoot = true;
        yield return new WaitForSeconds(waitTime);
        lockLeftFoot = false;
    }

    private IEnumerator ELockRFoot(float waitTime)
    {
        lockRightFoot = true;
        yield return new WaitForSeconds(waitTime);
        lockRightFoot = false;
    }

    //a callback for calculating IK
    protected virtual void OnAnimatorIK(int layerIdx)
    {
        if (layerIdx != m_animatorIkPassLayerIndex) return;

        Vector3 ikLeftPos = m_animator.GetIKPosition(AvatarIKGoal.LeftFoot);
        Vector3 ikRightPos = m_animator.GetIKPosition(AvatarIKGoal.RightFoot);

        Quaternion ikLeftRot = m_animator.GetIKRotation(AvatarIKGoal.LeftFoot);
        Quaternion ikRightRot = m_animator.GetIKRotation(AvatarIKGoal.RightFoot);

        if (m_lockLeftFoot)
        {
            ikLeftPos = m_leftFootLockedPos;
            ikLeftRot = m_leftFootLockedRot;
        }
        else
        {
            m_leftFootLockedPos = ikLeftPos;
            m_leftFootLockedRot = ikLeftRot;
        }

        if (m_lockRightFoot)
        {
            ikRightPos = m_rightFootLockedPos;
            ikRightRot = m_rightFootLockedRot;
        }
        else
        {
            m_rightFootLockedPos = ikRightPos;
            m_rightFootLockedRot = ikRightRot;
        }

        if (m_prevLeftLock != m_lockLeftFoot)
        {
            m_prevLeftLock = m_lockLeftFoot;
            
            Vector3 distVec = ikLeftPos - m_leftFootTargetPos;
            float dist = distVec.magnitude;

            dist = Mathf.Min(dist, m_maxIkDifferenceDistance);

            if (dist > 0.01f && m_transitionSmoothness > 0.01f)
            {
                m_currLeftLerp = 0;
                m_currLeftLerpDuration = dist / m_transitionSmoothness;
                m_currLeftLerpEndTime = Time.time + m_currLeftLerpDuration;
                m_currLeftStartPos = m_leftFootTargetPos;
                m_currLeftStartRot = m_leftFootTargetRot;
            }
            else
            {
                m_currLeftLerp = 1;
                m_currLeftLerpDuration = 1;
            }
        }

        if (m_prevRightLock != m_lockRightFoot)
        {
            m_prevRightLock = m_lockRightFoot;

            Vector3 distVec = ikRightPos - m_rightFootTargetPos;
            float dist = distVec.magnitude;

            dist = Mathf.Min(dist, m_maxIkDifferenceDistance);

            if (dist > 0.01f && m_transitionSmoothness > 0.01f)
            {
                m_currRightLerp = 0;
                m_currRightLerpDuration = dist / m_transitionSmoothness;
                m_currRightLerpEndTime = Time.time + m_currRightLerpDuration;
                m_currRightStartPos = m_rightFootTargetPos;
                m_currRightStartRot = m_rightFootTargetRot;
            }
            else
            {
                m_currRightLerp = 1;
                m_currRightLerpDuration = 1;
            }
        }

        if (m_currLeftLerpEndTime > Time.time) m_currLeftLerp = 1 - (m_currLeftLerpEndTime - Time.time) / m_currLeftLerpDuration;
        else m_currLeftLerp = 1;

        if (m_currRightLerpEndTime > Time.time) m_currRightLerp = 1 - (m_currRightLerpEndTime - Time.time) / m_currRightLerpDuration;
        else m_currRightLerp = 1;

        Vector3 leftDist = m_currLeftStartPos - m_leftUpperLeg.position;
        if (leftDist.magnitude > m_maxIkDifferenceDistance) m_currLeftStartPos = m_leftUpperLeg.position + Vector3.ClampMagnitude(leftDist, m_maxIkDifferenceDistance);

        Vector3 rightDist = m_currRightStartPos - m_rightUpperLeg.position;
        if (rightDist.magnitude > m_maxIkDifferenceDistance) m_currRightStartPos = m_rightUpperLeg.position + Vector3.ClampMagnitude(rightDist, m_maxIkDifferenceDistance);

        m_leftFootTargetPos = Vector3.Lerp(m_currLeftStartPos, ikLeftPos, m_currLeftLerp);
        m_leftFootTargetPos += transform.up * FootHeightCurve.Evaluate(m_currLeftLerp) * stepHeight;
        m_rightFootTargetPos = Vector3.Lerp(m_currRightStartPos, ikRightPos, m_currRightLerp);
        m_rightFootTargetPos += transform.up * FootHeightCurve.Evaluate(m_currRightLerp) * stepHeight;

        m_animator.SetIKPosition(AvatarIKGoal.LeftFoot, m_leftFootTargetPos);
        m_animator.SetIKPosition(AvatarIKGoal.RightFoot, m_rightFootTargetPos);
        m_animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, m_ikWeight);
        m_animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, m_ikWeight);

        if(m_lockRotationsAsWellWhenLocking)
        {
            m_leftFootTargetRot = Quaternion.Slerp(m_currLeftStartRot, ikLeftRot, m_currLeftLerp);
            m_rightFootTargetRot = Quaternion.Slerp(m_currRightStartRot, ikRightRot, m_currRightLerp);

            m_animator.SetIKRotation(AvatarIKGoal.LeftFoot, m_leftFootTargetRot);
            m_animator.SetIKRotation(AvatarIKGoal.RightFoot, m_rightFootTargetRot);
            m_animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, m_ikWeight);
            m_animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, m_ikWeight);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_leftFootTargetPos, 0.1f);
            Gizmos.DrawWireSphere(m_rightFootTargetPos, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(m_leftFootLockedPos, 0.1f);
            Gizmos.DrawWireSphere(m_rightFootLockedPos, 0.1f);
        }
    }
}
