using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovementData : MonoBehaviour
{
    [field: SerializeField] public Vector3 Direction { get; set; }
    [field: SerializeField] public Vector3 PointOfInterest { get; set; }
}
