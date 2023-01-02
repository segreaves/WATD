using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IShootable
{
    float Shoot(Vector3 aimDirection, float currentPower);
    UnityEvent OnShoot { get; set; }
}
