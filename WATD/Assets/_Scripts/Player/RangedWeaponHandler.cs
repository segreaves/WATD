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
    [field: SerializeField] public UnityEvent OnShootFailed { get; set; }
    [field: SerializeField] private MultiAimConstraint AimRig;
    public RangedWeapon ActiveWeaponInfo { get; private set; }
    public GameObject CurrentWeapon { get; private set; }
    private IShootable shootable;
    private Vector3 AimDirection;
    private float chargeTimer;
    private bool chargeTimerOn;
    private bool readyToShoot;
    private int bulletsShot;
    private bool triggerPressed;

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
        ActiveWeaponInfo = RangedWeapons[index];
        if (CurrentWeapon != null)
        {
            Destroy(CurrentWeapon);
        }
        CurrentWeapon = Instantiate(ActiveWeaponInfo.weaponData.WeaponPrefab, ActiveWeaponInfo.holster.transform.position, ActiveWeaponInfo.holster.transform.rotation);
        CurrentWeapon.SetActive(true);
        CurrentWeapon.transform.SetParent(ActiveWeaponInfo.holster.transform, true);
        // Attach gun to holster
        AttachToHolster();
        ReadyToShoot();
        // Set weapon data in gun controller
        shootable = CurrentWeapon.GetComponent<IShootable>();
        if (shootable == null) { return; }
        shootable.SetWeaponData(ActiveWeaponInfo.weaponData);
    }

    public void AttachToHand()
    {
        if (CurrentWeapon == null) { return; }
        CurrentWeapon.transform.SetParent(ActiveWeaponInfo.hand.transform, false);
    }

    public void AttachToHolster()
    {
        if (CurrentWeapon == null) { return; }
        CurrentWeapon.transform.SetParent(ActiveWeaponInfo.holster.transform, false);
    }

    public void StartAiming()
    {
        AimRig.weight = 1f;
    }

    public void AimWeapon(Vector3 aimDirection)
    {
        AimDirection = aimDirection;
        AimDirection.y = 0f;
    }

    public void StopAiming()
    {
        AimRig.weight = 0f;
    }

    private void ReadyToShoot()
    {
        readyToShoot = true;
    }

    public void PullTrigger(float power)
    {
        Debug.Log("Trigger pressed.");
        if (power >= ActiveWeaponInfo.weaponData.WeaponCost)
        {
            // Pull trigger on weapon
        }
        else
        {
            // Play can't shoot sound
        }
    }

    public void ReleaseTrigger()
    {
        Debug.Log("Trigger released.");
        // Release trigger on weapon
    }

    public bool Shoot_(Vector3 aimDirection, float power)
    {
        if (CurrentWeapon == null) { return false; }
        if (power < ActiveWeaponInfo.weaponData.WeaponCost)
        {
            // Play can't shoot sound
            return false;
        }
        if (shootable == null) { return false; }
        shootable.Shoot(BulletSpawn, aimDirection);
        OnShoot.Invoke(ActiveWeaponInfo.weaponData.WeaponCost);
        return true;
    }

    public void Shoot()
    {
        if (readyToShoot == false) { return; }
        readyToShoot = false;
        bulletsShot = 0;
        SendBullet();
        // Invoke ResetShot function (if not already invoked)
        Invoke("ResetShot", ActiveWeaponInfo.weaponData.TimeBetweenShooting);
    }

    private void SendBullet()
    {
        // Calculate spread
        float spread = Random.Range(-ActiveWeaponInfo.weaponData.Spread, ActiveWeaponInfo.weaponData.Spread);
        // Shooting direction with spread
        Vector3 directionWithSpread = Quaternion.Euler(0f, spread, 0f) * AimDirection;
        // Instantiate bullet
        GameObject currentBullet = Instantiate(ActiveWeaponInfo.weaponData.Bullet, BulletSpawn.transform.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread;
        // Instantiate muzzle flash
        if (ActiveWeaponInfo.weaponData.MuzzleFlash != null)
        {
            Instantiate(ActiveWeaponInfo.weaponData.MuzzleFlash, BulletSpawn.transform.position, currentBullet.transform.rotation);
        }
        bulletsShot++;
        // If more than one BulletsPerTap make sure to repeat Shoot function
        if (bulletsShot < ActiveWeaponInfo.weaponData.BulletsPerTap)
        {
            Invoke("SendBullet", ActiveWeaponInfo.weaponData.TimeBetweenShots);
        }
    }
}
