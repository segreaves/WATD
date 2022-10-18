using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [field: SerializeField] private LayerMask damageLayer;
    [field: SerializeField] private List<Weapon> Weapons;
    [field: SerializeField] public Weapon currentWeapon { get; private set; }
    public int attackIndex { get; private set; }
    private float resetTimer;
    [SerializeField] private bool showGizmos = true;

    void Start()
    {
        SetActiveWeapon(0);
    }

    private void Update()
    {
        // Uncomment this line to make attack index reset after a specified duration
        //AttackIndexReset()
    }

    public void SetActiveWeapon(int index)
    {
        // Set all blade data from array index into current blade
        if (Weapons[index] == null) { return; }
        currentWeapon = Weapons[index];
    }

    public void BladeEnable()
    {
        currentWeapon?.model.SetActive(true);
    }

    public void BladeDisable()
    {
        currentWeapon?.model.SetActive(false);
    }

    public void SetAttackIndex(int index, float resetTime)
    {
        attackIndex = index;
        resetTimer = resetTime;
    }

    public void IncrementAttackIndex()
    {
        if (attackIndex >= currentWeapon.weaponData.AttackAnimations.Count - 1)
        {
            attackIndex = 0;
            return;
        }
        attackIndex++;
        // Uncomment this line to make attack index reset after 0.25f
        //SetAttackIndex(attackIndex, 0.25f);
    }

    private void AttackIndexReset()
    {
        if (attackIndex > 0)
        {
            resetTimer -= Time.deltaTime;
            if (resetTimer <= 0)
            {
                attackIndex = 0;
            }
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
