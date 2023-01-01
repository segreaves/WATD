using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

using UnityEngine.Animations.Rigging;

public class RangedWeaponHandler : MonoBehaviour
{
    [field: SerializeField] private List<RangedWeapon> RangedWeapons;
    [field: SerializeField] private MultiAimConstraint aimRig;
    [field: SerializeField] public UnityEvent<float> OnShoot { get; set; }
    public RangedWeapon ActiveWeaponType { get; private set; }
    public GameObject ActiveWeapon { get; private set; }

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
        ActiveWeapon = Instantiate(ActiveWeaponType.weaponPrefab, ActiveWeaponType.holster.transform.position, ActiveWeaponType.holster.transform.rotation);
        if (ActiveWeapon == null) { return; }
        ActiveWeapon.SetActive(false);
        ActiveWeapon.transform.SetParent(ActiveWeaponType.holster.transform, true);
        ActiveWeapon.SetActive(true);
        AttachToHolster();
    }

    public void AttachToHand()
    {
        if (ActiveWeapon == null) { return; }
        ActiveWeapon.transform.SetParent(ActiveWeaponType.hand.transform, false);
    }

    public void AttachToHolster()
    {
        if (ActiveWeapon == null) { return; }
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

    public void Shoot(float power)
    {
        if (ActiveWeapon == null) { return; }
        var shootable = ActiveWeapon.GetComponent<IShootable>();
        if (shootable == null) { return; }
        float powerCost = shootable.Shoot(power);
        OnShoot.Invoke(powerCost);
    }
}
