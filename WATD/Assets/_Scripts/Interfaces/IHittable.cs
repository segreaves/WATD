using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IHittable
{
    void GetHit(int damage, GameObject damageDealer);
    UnityEvent OnGetHit { get; set; }
    UnityEvent OnDie { get; set; }
}
