using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponHandler : MonoBehaviour
{
    [field: SerializeField] private LayerMask damageLayer;
    [field: SerializeField] private List<Blade> Blades;
    [field: SerializeField] public Blade currentBlade { get; private set; }
    [field: SerializeField] public AnimationCurve WeaponExtendCurve { get; private set; }
    [SerializeField] private GameObject Weapon;
    [SerializeField] private GameObject handObject;
    [SerializeField] private GameObject holsterObject;
    public int attackIndex { get; private set; }
    public bool weaponEnabled { get; private set; }
    private float weaponExtendTimer;
    [field: SerializeField] private float transitionDuration = 0.2f;
    private Vector3 targetDimensions;
    private Vector3 dampingVelocity;
    [SerializeField] private bool showGizmos = true;

    void Start()
    {
        SetActiveBlade(0);
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
                currentBlade.model.transform.localScale = targetDimensions * curveValue;
            }
            else
            {
                float curveValue = WeaponExtendCurve.Evaluate(weaponExtendTimer / transitionDuration);
                if (curveValue < 0.05f) curveValue = 1f;
                currentBlade.model.transform.localScale = targetDimensions * curveValue;
            }
            weaponExtendTimer -= Time.deltaTime;
        }
    }

    public void SetActiveBlade(int index)
    {
        // Set all blade data from array index into current blade
        if (Blades == null) { return; }
        if (index < 0 || index >= Blades.Count) { return; }
        currentBlade = Blades[index];
        // Set weapon dimensions
        targetDimensions.x = currentBlade.bladeData.xDim;
        targetDimensions.y = currentBlade.bladeData.yDim;
        targetDimensions.z = currentBlade.bladeData.zDim;
        Weapon.gameObject.transform.SetParent(holsterObject.transform, false);
    }

    public void WeaponOn()
    {
        if (currentBlade == null) { return; }
        if (weaponEnabled == true) { return; }
        StartCoroutine(EWeaponOn());
    }

    public void WeaponOff()
    {
        if (currentBlade == null) { return; }
        if (weaponEnabled == false) { return; }
        StartCoroutine(EWeaponOff());
    }

    IEnumerator EWeaponOn()
    {
        weaponEnabled = true;
        currentBlade.model.transform.localScale = Vector3.zero;
        currentBlade.model.SetActive(true);
        weaponExtendTimer = transitionDuration;
        Weapon.gameObject.transform.SetParent(handObject.transform, false);
        yield return new WaitForSeconds(transitionDuration);
        if (weaponEnabled == true)
        {
            currentBlade.model.transform.localScale = targetDimensions;
        }
    }

    IEnumerator EWeaponOff()
    {
        weaponEnabled = false;
        weaponExtendTimer = transitionDuration;
        Weapon.gameObject.transform.SetParent(holsterObject.transform, false);
        yield return new WaitForSeconds(transitionDuration);
        if (weaponEnabled == false)
        {
            currentBlade.model.SetActive(false);
            currentBlade.model.transform.localScale = Vector3.zero;
        }
    }

    public void IncrementAttackIndex()
    {
        if (attackIndex < currentBlade.bladeData.AttackAnimations.Count - 1)
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
        Collider[] collisions = Physics.OverlapCapsule(GetDamageCapsuleStart(), GetDamageCapsuleEnd(), currentBlade.bladeData.HitCapsuleRadius, damageLayer);
        foreach (Collider collision in collisions)
        {
            var damageable = collision.GetComponent<IHittable>();
            if (damageable == null) { return; }
            damageable.GetHit(currentBlade.bladeData.Damage, gameObject);
        }
    }

    private Vector3 GetDamageCapsuleStart()
    {
        return transform.position + currentBlade.bladeData.HitCapsuleForwardOffset * transform.forward;
    }

    private Vector3 GetDamageCapsuleEnd()
    {
        return transform.position + (currentBlade.bladeData.HitCapsuleForwardOffset + (currentBlade.bladeData.HitCapsuleHeight - 1f)) * transform.forward;
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos == false) { return; }
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GetDamageCapsuleStart(), currentBlade.bladeData.HitCapsuleRadius);
            Gizmos.DrawWireSphere(GetDamageCapsuleEnd(), currentBlade.bladeData.HitCapsuleRadius);
        }
    }
}
