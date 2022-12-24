using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IShootable
{
    void StartShootting();
    void StopShootting();
    UnityEvent OnShoot { get; set; }
}
