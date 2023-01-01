using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunController : MonoBehaviour, IShootable
{
    [field: SerializeField] private GameObject Bullet;
    [field: SerializeField] private GameObject BulletSpawn { get; set; }
    [field: SerializeField] private RangedWeaponSO WeaponData;
    [field: SerializeField] public UnityEvent OnShoot { get; set; }
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

    public float Shoot(float power)
    {
        if (readyToShoot == false) { return 0f; }
        if (power < WeaponData.PowerCost) { return 0f; }
        readyToShoot = false;
        bulletsShot = 0;
        SendBullet();
        // Invoke ResetShot function (if not already invoked)
        Invoke("ResetShot", WeaponData.TimeBetweenShooting);
        return WeaponData.PowerCost;
    }

    private void SendBullet()
    {
        // Shooting direction without spread
        Vector3 directionWithoutSpread = BulletSpawn.transform.forward;
        directionWithoutSpread.y = 0f;
        // Calculate spread
        float spread = Random.Range(-WeaponData.Spread, WeaponData.Spread);
        // Shooting direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(spread, 0f, 0f);
        // Instantiate bullet
        GameObject currentBullet = Instantiate(Bullet, BulletSpawn.transform.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread;
        // Instantiate muzzle flash
        if (WeaponData.MuzzleFlash != null)
        {
            Instantiate(WeaponData.MuzzleFlash, BulletSpawn.transform.position, currentBullet.transform.rotation);
        }
        // Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * WeaponData.ShootForce, ForceMode.Impulse);
        bulletsShot++;
        // If more than one BulletsPerTap make sure to repeat Shoot function
        if (bulletsShot < WeaponData.BulletsPerTap)
        {
            Invoke("SendBullet", WeaponData.TimeBetweenShots);
        }
    }
}
