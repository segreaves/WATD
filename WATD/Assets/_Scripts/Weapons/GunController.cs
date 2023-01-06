using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunController : MonoBehaviour, IShootable
{
    [field: SerializeField] private GameObject Bullet;
    [field: SerializeField] public UnityEvent OnTriggerPress { get; set; }
    [field: SerializeField] public UnityEvent OnTriggerRelease { get; set; }
    [field: SerializeField] public UnityEvent OnShoot { get; set; }
    private RangedWeaponSO WeaponData;
    private bool readyToShoot;
    private int bulletsShot;
    private bool triggerPressed;

    private void Awake()
    {
        readyToShoot = true;
    }

    private void Start()
    {
        readyToShoot = true;
    }

    private void Update()
    {
        if (triggerPressed == true && readyToShoot)
        {
            //Shoot(GameObject bulletSpawn, AimDirection)
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    public void Shoot(GameObject bulletSpawn, Vector3 aimDirection)
    {
        if (readyToShoot == false) { return; }
        readyToShoot = false;
        bulletsShot = 0;
        SendBullet(bulletSpawn, aimDirection);
        // Invoke ResetShot function (if not already invoked)
        Invoke("ResetShot", WeaponData.TimeBetweenShooting);
    }

    private void SendBullet(GameObject bulletSpawn, Vector3 aimDirection)
    {
        // Calculate spread
        float spread = Random.Range(-WeaponData.Spread, WeaponData.Spread);
        // Shooting direction with spread
        aimDirection.y = 0f;
        Vector3 directionWithSpread = Quaternion.Euler(0f, spread, 0f) * aimDirection;
        // Instantiate bullet
        GameObject currentBullet = Instantiate(Bullet, bulletSpawn.transform.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread;
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

    public void Trigger(bool pressed)
    {
        triggerPressed = pressed;
    }
}
