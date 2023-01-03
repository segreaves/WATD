using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IHittable
{
    void GetHit(int damage, Vector3 damageDirection);
    UnityEvent OnGetHit { get; set; }
    UnityEvent OnDie { get; set; }
}
