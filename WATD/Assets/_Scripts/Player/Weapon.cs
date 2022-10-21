using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Weapon
{
    [SerializeField] public string weaponName;
    [SerializeField] public GameObject model;
    [SerializeField] public MeleeWeaponSO weaponData;
}
