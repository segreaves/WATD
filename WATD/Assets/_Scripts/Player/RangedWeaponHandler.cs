using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Animations.Rigging;

public class RangedWeaponHandler : MonoBehaviour
{
    [field: SerializeField] private List<RangedWeapon> RangedWeapons;
    //[field: SerializeField] private GameObject BulletSpawn;
    [field: SerializeField] private MultiAimConstraint aimRig;
    private RangedWeapon currentRanged;
    

    void Start()
    {
        SetActiveRangedWeapon(0);
    }

    public void SetActiveRangedWeapon(int index)
    {
        // Set all weapon data from array index into current weapon
        if (RangedWeapons == null) { return; }
        if (index < 0 || index >= RangedWeapons.Count) { return; }
        currentRanged = RangedWeapons[index];
        AttachToHolster();
    }

    public void AttachToHand()
    {
        currentRanged.body.transform.SetParent(currentRanged.hand.transform, false);
    }

    public void AttachToHolster()
    {
        currentRanged.body.transform.SetParent(currentRanged.holster.transform, false);
    }

    public void StartAiming()
    {
        aimRig.weight = 1f;
    }

    public void StopAiming()
    {
        aimRig.weight = 0f;
    }
}
