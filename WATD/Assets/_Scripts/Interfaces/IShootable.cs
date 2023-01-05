using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IShootable
{
    void Shoot(GameObject bulletSpawn, Vector3 aimDirection, LayerMask damageLayer);
    void SetWeaponData(RangedWeaponSO weaponData);
}
