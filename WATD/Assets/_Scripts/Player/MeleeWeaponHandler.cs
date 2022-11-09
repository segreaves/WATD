using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponHandler : MonoBehaviour
{
    [field: SerializeField] private LayerMask damageLayer;
    [field: SerializeField] private List<MeleeWeapon> MeleeWeapons;
    [field: SerializeField] public MeleeWeapon currentWeapon { get; private set; }
    public int attackIndex { get; private set; }
    [SerializeField] private bool showGizmos = true;

    void Start()
    {
        SetActiveMeleeWeapon(0);
    }

    public void SetActiveMeleeWeapon(int index)
    {
        // Set all weapon data from array index into current weapon
        if (MeleeWeapons == null) { return; }
        if (index < 0 || index >= MeleeWeapons.Count) { return; }
        currentWeapon = MeleeWeapons[index];
        AttachToHolster();
    }

    public void AttachToHand()
    {
        currentWeapon.body.transform.SetParent(currentWeapon.hand.transform, false);
    }

    public void AttachToHolster()
    {
        currentWeapon.body.transform.SetParent(currentWeapon.holster.transform, false);
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
        // Instantiate capsule collider in front of player with characteristics from the weapon damage info
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
