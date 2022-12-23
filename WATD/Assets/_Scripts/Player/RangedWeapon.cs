using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RangedWeapon
{
    [SerializeField] public GameObject body;
    [SerializeField] public GameObject hand;
    [SerializeField] public GameObject holster;
    [SerializeField] public GameObject bulletSpawn;
    [SerializeField] public RangedWeaponSO weaponData;
}
