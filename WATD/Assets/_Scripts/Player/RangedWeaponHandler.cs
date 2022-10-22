using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeaponHandler : MonoBehaviour
{
    [SerializeField] private GameObject Weapon;
    [SerializeField] private GameObject handObject;
    [SerializeField] private GameObject holsterObject;
    public bool weaponEnabled { get; private set; }
    
    public void Shoot()
    {
        Debug.Log("Shot");
    }

    public void WeaponOn()
    {
        if (weaponEnabled == true) { return; }
        weaponEnabled = true;
        // Draw weapon
        Weapon.gameObject.transform.SetParent(handObject.transform, false);
    }

    public void WeaponOff()
    {
        if (weaponEnabled == false) { return; }
        weaponEnabled = false;
        // Holster
        Weapon.gameObject.transform.SetParent(holsterObject.transform, false);
    }
}
