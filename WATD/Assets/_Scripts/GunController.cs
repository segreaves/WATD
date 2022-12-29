using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunController : MonoBehaviour, IShootable
{
    [field: SerializeField] public UnityEvent OnShoot { get; set; }
    [field: SerializeField] private GameObject BulletSpawn { get; set; }
    private bool shouldShoot, shooting, readyToShoot;
    private int magazineSize = 3, bulletsLeft, bulletsShot;
    private bool allowInvoke = true;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        // Shooting
        if (readyToShoot && shooting && bulletsLeft > 0)
        {
            bulletsShot = 0;
            //Shoot();
        }
    }

    public void StartShootting()
    {
        //Debug.Log("GunController.cs shooting enabled");
    }

    public void StopShootting()
    {
        //Debug.Log("GunController.cs shooting disabled");
    }

    public void SetShooting(bool shoot)
    {
        shouldShoot = shoot;
        shooting = shouldShoot;
    }

    /*private void ResetShot()
    {
        shooting = currentRanged.weaponData.AllowButtonHold && shouldShoot;
        readyToShoot = true;
        allowInvoke = true;
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
        GameObject currentBullet = Instantiate(currentRanged.weaponData.bullet, BulletSpawn.transform.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread;
        // Instantiate muzzle flash
        if (currentRanged.weaponData.MuzzleFlash != null)
        {
            Instantiate(currentRanged.weaponData.MuzzleFlash, BulletSpawn.transform.position, currentBullet.transform.rotation);
        }
        // Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * currentRanged.weaponData.ShootForce, ForceMode.Impulse);
        // Invoke ResetShot function (if not already invoked)
        if (allowInvoke)
        {
            Invoke("ResetShot", currentRanged.weaponData.TimeBetweenShooting);
            allowInvoke = false;
        }
        bulletsLeft--;
        bulletsShot++;
        // If more than one BulletsPerTap make sure to repeat Shoot function
        if (bulletsShot < currentRanged.weaponData.BulletsPerTap)
        {
            Invoke("Shoot", currentRanged.weaponData.TimeBetweenShots);
        }
    }*/
}
