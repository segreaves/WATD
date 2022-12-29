using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UnityEngine.Animations.Rigging;

public class RangedWeaponHandler : MonoBehaviour
{
    [field: SerializeField] private List<RangedWeapon> RangedWeapons;
    [field: SerializeField] private MultiAimConstraint aimRig;
    public RangedWeapon ActiveWeaponType { get; private set; }
    private GameObject ActiveWeapon;

    void Start()
    {
        SetActiveRangedWeapon(0);
    }

    public void SetActiveRangedWeapon(int index)
    {
        // Set all weapon data from array index into current weapon
        if (RangedWeapons == null) { return; }
        if (index < 0 || index >= RangedWeapons.Count) { return; }
        ActiveWeaponType = RangedWeapons[index];
        ActiveWeapon?.SetActive(false);
        ActiveWeapon = Instantiate(ActiveWeaponType.weaponData.WeaponPrefab, ActiveWeaponType.holster.transform.position, ActiveWeaponType.holster.transform.rotation);
        ActiveWeapon.transform.SetParent(ActiveWeaponType.holster.transform, true);
        ActiveWeapon.SetActive(true);
        AttachToHolster();
    }

    public void AttachToHand()
    {
        ActiveWeapon.transform.SetParent(ActiveWeaponType.hand.transform, false);
    }

    public void AttachToHolster()
    {
        ActiveWeapon.transform.SetParent(ActiveWeaponType.holster.transform, false);
    }

    public void StartAiming()
    {
        aimRig.weight = 1f;
    }

    public void StopAiming()
    {
        aimRig.weight = 0f;
    }

    public void Shoot()
    {
        var shootable = ActiveWeapon.GetComponent<IShootable>();
        if (shootable == null) { return; }
        shootable.StartShootting();
        //Debug.Log("RangedWeaponHandler enabled");
    }

    public void StopShooting()
    {
        var shootable = ActiveWeapon.GetComponent<IShootable>();
        if (shootable == null) { return; }
        shootable.StopShootting();
        //Debug.Log("RangedWeaponHandler disabled");
    }
}
