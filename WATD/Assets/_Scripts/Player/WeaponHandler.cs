using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [field: SerializeField] private LayerMask damageLayer;
    [field: SerializeField] private List<Weapon> Weapons;
    [field: SerializeField] public Weapon currentWeapon { get; private set; }
    [field: SerializeField] public AnimationCurve WeaponExtendCurve { get; private set; }
    [SerializeField] public int attackIndex;
    public bool weaponEnabled { get; private set; }
    private float weaponExtendTimer;
    [field: SerializeField] private float transitionDuration = 0.2f;
    private Vector3 targetDimensions;
    private Vector3 dampingVelocity;
    [SerializeField] private bool showGizmos = true;

    void Start()
    {
        SetActiveWeapon(0);
    }

    private void Update()
    {
        // Uncomment this line to make attack index reset after a specified duration
        //AttackIndexReset()
        if (weaponExtendTimer > 0)
        {
            if (weaponEnabled)
            {
                float curveValue = WeaponExtendCurve.Evaluate((1 - weaponExtendTimer) / transitionDuration);
                if (curveValue > 0.95f) curveValue = 1f;
                currentWeapon.model.transform.localScale = targetDimensions * curveValue;
            }
            else
            {
                float curveValue = WeaponExtendCurve.Evaluate(weaponExtendTimer / transitionDuration);
                if (curveValue < 0.05f) curveValue = 1f;
                currentWeapon.model.transform.localScale = targetDimensions * curveValue;
            }
            weaponExtendTimer -= Time.deltaTime;
        }
    }

    public void SetActiveWeapon(int index)
    {
        // Set all blade data from array index into current blade
        if (Weapons[index] == null) { return; }
        currentWeapon = Weapons[index];
        // Set weapon dimensions
        targetDimensions.x = currentWeapon.weaponData.xDim;
        targetDimensions.y = currentWeapon.weaponData.yDim;
        targetDimensions.z = currentWeapon.weaponData.zDim;
    }

    public void WeaponOn()
    {
        if (currentWeapon == null) { return; }
        if (weaponEnabled == true) { return; }
        StartCoroutine(EWeaponOn());
    }

    public void WeaponOff()
    {
        if (currentWeapon == null) { return; }
        if (weaponEnabled == false) { return; }
        StartCoroutine(EWeaponOff());
    }

    IEnumerator EWeaponOn()
    {
        weaponEnabled = true;
        currentWeapon.model.transform.localScale = Vector3.zero;
        currentWeapon.model.SetActive(true);
        weaponExtendTimer = transitionDuration;
        yield return new WaitForSeconds(transitionDuration);
        if (weaponEnabled == true)
        {
            currentWeapon.model.transform.localScale = targetDimensions;
        }
    }

    IEnumerator EWeaponOff()
    {
        weaponEnabled = false;
        weaponExtendTimer = transitionDuration;
        yield return new WaitForSeconds(transitionDuration);
        if (weaponEnabled == false)
        {
            currentWeapon.model.SetActive(false);
            currentWeapon.model.transform.localScale = Vector3.zero;
        }
    }

    public void IncrementAttackIndex()
    {
        if (attackIndex < currentWeapon.weaponData.AttackAnimations.Count - 1)
        {
            attackIndex++;
        }
        else
        {
            attackIndex = 0;
        }
    }

    public void DamageFrame()
    {
        // Instantiate capsule collider in front of player with characteristics from the blade damage info
        Collider[] collisions = Physics.OverlapCapsule(GetDamageCapsuleStart(), GetDamageCapsuleEnd(), currentWeapon.weaponData.HitCapsuleRadius, damageLayer);
        foreach (Collider collision in collisions)
        {
            var damageable = collision.GetComponent<IHittable>();
            if (damageable == null) { return; }
            damageable.GetHit(currentWeapon.weaponData.Damage, gameObject);
        }
    }

    private Vector3 GetDamageCapsuleStart()
    {
        return transform.position + currentWeapon.weaponData.HitCapsuleForwardOffset * transform.forward;
    }

    private Vector3 GetDamageCapsuleEnd()
    {
        return transform.position + (currentWeapon.weaponData.HitCapsuleForwardOffset + (currentWeapon.weaponData.HitCapsuleHeight - 1f)) * transform.forward;
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos == false) { return; }
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GetDamageCapsuleStart(), currentWeapon.weaponData.HitCapsuleRadius);
            Gizmos.DrawWireSphere(GetDamageCapsuleEnd(), currentWeapon.weaponData.HitCapsuleRadius);
        }
    }
}
