using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunController : MonoBehaviour, IShootable
{
    [field: SerializeField] private GameObject Bullet;
    private RangedWeaponSO WeaponData;
    private bool readyToShoot;
    private int bulletsShot;

    private void Awake()
    {
        readyToShoot = true;
    }

    private void Start()
    {
        readyToShoot = true;
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    public void Shoot(GameObject bulletSpawn, Vector3 aimDirection, LayerMask damageLayer)
    {
        if (readyToShoot == false) { return; }
        readyToShoot = false;
        bulletsShot = 0;
        SendBullet(bulletSpawn, aimDirection, damageLayer);
        // Invoke ResetShot function (if not already invoked)
        Invoke("ResetShot", WeaponData.TimeBetweenShooting);
    }

    private void SendBullet(GameObject bulletSpawn, Vector3 aimDirection, LayerMask damageLayer)
    {
        // Calculate spread
        float spread = Random.Range(-WeaponData.Spread, WeaponData.Spread);
        // Shooting direction with spread
        aimDirection.y = 0f;
        Vector3 directionWithSpread = Quaternion.Euler(0f, spread, 0f) * aimDirection;
        // Instantiate bullet
        GameObject currentBullet = Instantiate(Bullet, bulletSpawn.transform.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread;
        var customProjectile = currentBullet.GetComponent<CustomProjectile>();
        if (customProjectile != null)
        {
            // Set whom the bullet can damage
            customProjectile.damageLayer = damageLayer;
        }
        // Instantiate muzzle flash
        if (WeaponData.MuzzleFlash != null)
        {
            Instantiate(WeaponData.MuzzleFlash, bulletSpawn.transform.position, currentBullet.transform.rotation);
        }
        bulletsShot++;
        // If more than one BulletsPerTap make sure to repeat Shoot function
        if (bulletsShot < WeaponData.BulletsPerTap)
        {
            Invoke("SendBullet", WeaponData.TimeBetweenShots);
        }
    }

    public void SetWeaponData(RangedWeaponSO weaponData)
    {
        WeaponData = weaponData;
    }
}
