using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeaponHandler : MonoBehaviour
{
    [field: SerializeField] private List<RangedWeapon> RangedWeapons;
    [field: SerializeField] public RangedWeapon currentWeapon { get; private set; }

    void Start()
    {
        SetActiveRangedWeapon(0);
    }

    public void SetActiveRangedWeapon(int index)
    {
        // Set all weapon data from array index into current weapon
        if (RangedWeapons == null) { return; }
        if (index < 0 || index >= RangedWeapons.Count) { return; }
        currentWeapon = RangedWeapons[index];
        AttachToHolster();
    }

    public void AttachToHand()
    {
        currentWeapon.body.transform.SetParent(currentWeapon.hand.transform, false);
    }

    public void AttachToHolster()
    {
        currentWeapon.body.transform.SetParent(currentWeapon.holster.transform, false);
    }
}
