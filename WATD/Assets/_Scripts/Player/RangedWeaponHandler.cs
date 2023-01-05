using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

using UnityEngine.Animations.Rigging;

public class RangedWeaponHandler : MonoBehaviour
{
    [field: SerializeField] private GameObject BulletSpawn;
    [field: SerializeField] private List<RangedWeapon> RangedWeapons;
    [field: SerializeField] public UnityEvent<float> OnShoot { get; set; }
    [field: SerializeField] private MultiAimConstraint aimRig;
    public RangedWeapon ActiveWeapon { get; private set; }
    public GameObject CurrentWeapon { get; private set; }
    public RangedWeaponSO WeaponData { get; private set; }

    void Start()
    {
        SetActiveRangedWeapon(0);
    }

    public void SetActiveRangedWeapon(int index)
    {
        // Set all weapon data from array index into current weapon
        if (RangedWeapons == null) { return; }
        if (index < 0 || index >= RangedWeapons.Count) { return; }
        if (RangedWeapons[index].weaponData == null) { return; }
        if (RangedWeapons[index].weaponData.WeaponPrefab == null) { return; }
        if (RangedWeapons[index].holster == null) { return; }
        if (RangedWeapons[index].hand == null) { return; }
        ActiveWeapon = RangedWeapons[index];
        WeaponData = ActiveWeapon.weaponData;
        if (CurrentWeapon != null)
        {
            Destroy(CurrentWeapon);
        }
        CurrentWeapon = Instantiate(WeaponData.WeaponPrefab, ActiveWeapon.holster.transform.position, ActiveWeapon.holster.transform.rotation);
        CurrentWeapon.SetActive(true);
        CurrentWeapon.transform.SetParent(ActiveWeapon.holster.transform, true);
        // Attach gun to holster
        AttachToHolster();
        // Set weapon data in gun controller
        var shootable = CurrentWeapon.GetComponent<IShootable>();
        if (shootable == null) { return; }
        shootable.SetWeaponData(WeaponData);
    }

    public void AttachToHand()
    {
        if (CurrentWeapon == null) { return; }
        CurrentWeapon.transform.SetParent(ActiveWeapon.hand.transform, false);
    }

    public void AttachToHolster()
    {
        if (CurrentWeapon == null) { return; }
        CurrentWeapon.transform.SetParent(ActiveWeapon.holster.transform, false);
    }

    public void StartAiming()
    {
        aimRig.weight = 1f;
    }

    public void StopAiming()
    {
        aimRig.weight = 0f;
    }

    public bool Shoot(Vector3 aimDirection, float power)
    {
        if (CurrentWeapon == null) { return false; }
        if (power < WeaponData.WeaponCost)
        {
            // Play can't shoot sound
            return false;
        }
        var shootable = CurrentWeapon.GetComponent<IShootable>();
        if (shootable == null) { return false; }
        shootable.Shoot(BulletSpawn, aimDirection, ActiveWeapon.damageLayer);
        OnShoot.Invoke(WeaponData.WeaponCost);
        return true;
    }
}
