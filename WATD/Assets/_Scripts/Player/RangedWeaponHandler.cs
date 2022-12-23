using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Animations.Rigging;

public class RangedWeaponHandler : MonoBehaviour
{
    [field: SerializeField] private List<RangedWeapon> RangedWeapons;
    [field: SerializeField] private GameObject BulletSpawn;
    [field: SerializeField] private int magazineSize = 3;
    [field: SerializeField] private MultiAimConstraint aimRig;
    private RangedWeapon currentRanged;
    private bool shouldShoot, shooting, readyToShoot;
    private int bulletsLeft, bulletsShot;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    void Start()
    {
        SetActiveRangedWeapon(0);
    }

    private void Update()
    {
        // Shooting
        if (readyToShoot && shooting && bulletsLeft > 0)
        {
            bulletsShot = 0;
            Shoot();
        }
        // Check if allowed to hold down shoot button
        shooting = currentRanged.weaponData.AllowButtonHold && shouldShoot;
    }

    private void Shoot()
    {
        readyToShoot = false;
        // Shooting direction without spread
        Vector3 directionWithoutSpread = BulletSpawn.transform.forward;
        // Calculate spread
        float spread = Random.Range(-currentRanged.weaponData.Spread, currentRanged.weaponData.Spread);
        // Shooting direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(spread, 0f, 0f);
        // Instantiate bullet
        GameObject currentBullet = Instantiate(currentRanged.weaponData.projectile, BulletSpawn.transform.position, Quaternion.identity);
        bulletsLeft--;
        bulletsShot++;
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

    public void SetShooting(bool shoot)
    {
        shouldShoot = shoot;
    }
}
