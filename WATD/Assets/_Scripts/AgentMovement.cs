using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AgentMovement : MonoBehaviour
{
    protected Rigidbody rigidBody;

    [field: SerializeField] public MovementDataSO MovementData { get; set; }

    [SerializeField] protected float currentVelocity;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void MoveAgent(Vector2 movementInput)
    {
        Vector3 movement = new Vector3(movementInput.x, 0f, movementInput.y);
        rigidBody.velocity = movement.normalized * currentVelocity;
    }
}
