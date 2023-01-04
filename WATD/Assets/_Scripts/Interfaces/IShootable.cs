using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IShootable
{
    void Shoot(Vector3 aimDirection);
    RangedWeaponSO GetWeaponData();
    UnityEvent OnShoot { get; set; }
}
