using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseAction : AIAction
{
    public override void Enter()
    {
        Agent.enabled = true;
    }

    public override void Exit()
    {
        Agent.enabled = false;
    }

    public override void Tick()
    {
        // Look
        aiMovementData.PointOfInterest = enemyBrain.Target.transform.position;
        enemyBrain.LookAt(aiMovementData.PointOfInterest);
    }

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest)
    {
        if (enemyBrain.Target == null)
        {
            return (danger, interest);
        }
        else
        {
            Vector3 directionToTarget = GetDirectionToTarget();
            directionToTarget.y = 0f;
            for (int i = 0; i < interest.Length; i++)
            {
                float result = Vector3.Dot(directionToTarget.normalized, Directions.eightDirections[i]);

                // Accept only directions towards target
                if (result > 0)
                {
                    float valueToPutIn = result;
                    if (valueToPutIn > interest[i])
                    {
                        interest[i] = valueToPutIn;
                    }

                }
            }
            return (danger, interest);
        }
    }

    private Vector3 GetDirectionToTarget()
    {
        Agent.transform.position = transform.position;
        Agent.velocity = Controller.velocity;
        if (Agent.isOnNavMesh && Agent.enabled)
        {
            // Use NavMesh
            Agent.destination = enemyBrain.Target.transform.position;
            return Agent.desiredVelocity.normalized;
        }
        else
        {
            // Try straight line to target
            var direction = enemyBrain.Target.transform.position - transform.position;
            return direction.normalized;
        }
    }
}
