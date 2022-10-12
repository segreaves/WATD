using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IAgentInput
{
    UnityEvent<Vector3> OnMovement { get; set; }
    UnityEvent<Vector3> OnFaceDirection { get; set; }
}
