using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AgentMovement : MonoBehaviour
{
    protected Rigidbody rigidBody;

    [SerializeField] protected float currentVelocity = 3f;

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
