using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIK_Simple : MonoBehaviour
{
    public bool enableFeetIK;
    private Animator Animator;
    private Vector3 leftFootPosition, rightFootPosition, leftFootIKPosition, rightFootIKPosition;
    private Quaternion leftFootIKRotation, rightFootIKRotation;
    private float lastPelvisPositionY, lastLeftFootPositionY, lastRightFootPositionY;
    [SerializeField] [Range(0f, 2f)] private float heightFromGroundRaycast = 1.14f;
    [SerializeField] [Range(0f, 2f)] private float raycastDownDistance = 1.5f;
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private float pelvisOffset = 0f;
    [SerializeField] [Range(0f, 1f)] float pelvisUpAndDownSpeed = 0.28f;
    [SerializeField] [Range(0f, 1f)] private float feetToIKPositionSpeed = 0.5f;

    public string leftFootAnimVariableName = "LeftFootCurve";
    public string rightFootAnimVariableName = "RightFootCurve";

    public bool useProIKFeature = false;
    public bool showSolverDebug = true;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        // Update AdjustFeetTarget method and find the position of each foot inside our solver position
        if (enableFeetIK == false) { return; }
        if (Animator == null) { return; }

        AdjustFootTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
        AdjustFootTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

        // Find and raycast to the ground to find positions
        FeetPositionSolver(rightFootPosition, ref rightFootIKPosition, ref rightFootIKRotation);
        FeetPositionSolver(leftFootPosition, ref leftFootIKPosition, ref leftFootIKRotation);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (enableFeetIK == false) { return; }
        if (Animator == null) { return; }

        MovePelvisHeight();
        // Right foot IK position and rotation
        Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
        if (useProIKFeature)
        {
            Animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, Animator.GetFloat(rightFootAnimVariableName));
        }
        MoveFootToIKPoint(AvatarIKGoal.RightFoot, rightFootIKPosition, rightFootIKRotation, ref lastRightFootPositionY);
        // Left foot IK position and rotation
        Animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
        if (useProIKFeature)
        {
            Animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, Animator.GetFloat(leftFootAnimVariableName));
        }
        MoveFootToIKPoint(AvatarIKGoal.LeftFoot, leftFootIKPosition, leftFootIKRotation, ref lastLeftFootPositionY);
    }

    void MoveFootToIKPoint(AvatarIKGoal foot, Vector3 positionIKHolder, Quaternion rotationIKHolder, ref float lastFootPositionY)
    {
        Vector3 targetIKPosition = Animator.GetIKPosition(foot);
        if (positionIKHolder != Vector3.zero)
        {
            targetIKPosition = transform.InverseTransformPoint(targetIKPosition);
            positionIKHolder = transform.InverseTransformPoint(positionIKHolder);
            float yVariable = Mathf.Lerp(lastFootPositionY, positionIKHolder.y, feetToIKPositionSpeed);
            targetIKPosition.y += yVariable;
            lastFootPositionY = yVariable;
            targetIKPosition = transform.TransformPoint(targetIKPosition);
            Animator.SetIKRotation(foot, rotationIKHolder);
        }
        Animator.SetIKPosition(foot, targetIKPosition);
    }

    private void MovePelvisHeight()
    {
        if (rightFootIKPosition == Vector3.zero || leftFootIKPosition == Vector3.zero || lastPelvisPositionY == 0f)
        {
            lastPelvisPositionY = Animator.bodyPosition.y;
            return;
        }
        float leftOffsetPosition = leftFootIKPosition.y - transform.position.y;
        float rightOffsetPosition = rightFootIKPosition.y - transform.position.y;
        float totalOffset = leftOffsetPosition < rightOffsetPosition ? leftOffsetPosition : rightOffsetPosition;
        Vector3 newPelvisPosition = Animator.bodyPosition + Vector3.up * totalOffset;
        newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisUpAndDownSpeed);
        Animator.bodyPosition = newPelvisPosition;
        lastPelvisPositionY = Animator.bodyPosition.y;
    }

    private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIKPositions, ref Quaternion feetIKRotations)
    {
        // Raycast handling
        RaycastHit feetOutHit;
        if (showSolverDebug == true)
        {
            Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.yellow);
        }
        if (Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit, raycastDownDistance + heightFromGroundRaycast, environmentLayer))
        {
            feetIKPositions = fromSkyPosition;
            feetIKPositions.y = feetOutHit.point.y + pelvisOffset;
            feetIKRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;
            return;
        }
        feetIKPositions = Vector3.zero;
    }

    private void AdjustFootTarget(ref Vector3 feetPositions, HumanBodyBones foot)
    {
        feetPositions = Animator.GetBoneTransform(foot).position;
        feetPositions.y = transform.position.y + heightFromGroundRaycast;
    }
}
