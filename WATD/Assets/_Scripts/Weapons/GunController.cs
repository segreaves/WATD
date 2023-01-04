using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunController : MonoBehaviour, IShootable
{
    [field: SerializeField] private GameObject Bullet;
    [field: SerializeField] private LayerMask damageLayer;
    [field: SerializeField] private GameObject BulletSpawn { get; set; }
    [field: SerializeField] private RangedWeaponSO WeaponData;
    [field: SerializeField] public UnityEvent OnShoot { get; set; }
    private bool readyToShoot;
    private int bulletsShot;
    private Vector3 AimDirection;

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

    public void Shoot(Vector3 aimDirection)
    {
        if (readyToShoot == false) { return; }
        readyToShoot = false;
        bulletsShot = 0;
        AimDirection = aimDirection;
        SendBullet();
        // Invoke ResetShot function (if not already invoked)
        Invoke("ResetShot", WeaponData.TimeBetweenShooting);
    }

    private void SendBullet()
    {
        // Calculate spread
        float spread = Random.Range(-WeaponData.Spread, WeaponData.Spread);
        // Shooting direction with spread
        Vector3 directionWithSpread = Quaternion.Euler(0f, spread, 0f) * AimDirection;
        // Instantiate bullet
        GameObject currentBullet = Instantiate(Bullet, BulletSpawn.transform.position, Quaternion.identity);
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
            Instantiate(WeaponData.MuzzleFlash, BulletSpawn.transform.position, currentBullet.transform.rotation);
        }
        bulletsShot++;
        // If more than one BulletsPerTap make sure to repeat Shoot function
        if (bulletsShot < WeaponData.BulletsPerTap)
        {
            Invoke("SendBullet", WeaponData.TimeBetweenShots);
        }
    }

    public RangedWeaponSO GetWeaponData()
    {
        return(WeaponData);
    }
}
