using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private List<SteeringBehavior> steeringBehaviors;
    [SerializeField] private List<Detector> detectors;
    [SerializeField] private AIData aiData;
    [SerializeField] private float detectionDelay = 0.05f, aiUpdateDelay = 0.06f;
    [SerializeField] private ContextSolver movementDirectionSolver;

    //Inputs sent from the Enemy AI to the Enemy controller
    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;

    private Vector2 movementInput;

    bool following = false;

    private void Start()
    {
        //Detecting Player and Obstacles around
        InvokeRepeating("PerformDetection", 0, detectionDelay);
    }

    private void PerformDetection()
    {
        foreach (Detector detector in detectors)
        {
            detector.Detect(aiData);
        }
    }

    private void Update()
    {
        //Enemy AI movement based on Target availability
        if (aiData.currentTarget != null)
        {
            //Looking at the Target
            OnPointerInput?.Invoke(aiData.currentTarget.transform.position);
            if (following == false)
            {
                following = true;
                StartCoroutine(EChase());
            }
        }
        //Moving the Agent
        OnMovementInput?.Invoke(movementInput);
    }

    private IEnumerator EChase()
    {
        if (aiData.currentTarget == null)
        {
            //Stopping Logic
            Debug.Log("Stopping");
            movementInput = Vector2.zero;
            following = false;
            yield break;
        }
        else
        {
            //Chase logic
            Vector3 movement = movementDirectionSolver.GetDirectionToMove(steeringBehaviors, aiData);
            movementInput.x = movement.x;
            movementInput.y = movement.z;
            yield return new WaitForSeconds(aiUpdateDelay);
            StartCoroutine(EChase());
        }

    }
}
