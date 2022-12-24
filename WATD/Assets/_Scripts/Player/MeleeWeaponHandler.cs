using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponHandler : MonoBehaviour
{
    [field: SerializeField] private LayerMask damageLayer;
    [field: SerializeField] private List<MeleeWeapon> MeleeWeapons;
    [field: SerializeField] public MeleeWeapon currentMelee { get; private set; }
    public int attackIndex { get; private set; }
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private AnimationCurve activationCurve;
    [SerializeField] private float transitionSpeed = 5f;
    private GameObject WeaponBody;
    private bool isActivated;
    private bool isTransitioning;
    private float activationValue;

    void Start()
    {
        SetActiveMeleeWeapon(0);
    }

    private void Update()
    { 
        UpdateWeaponScale();
    }

    private void UpdateWeaponScale()
    {
        if (isTransitioning == true)
        {
            if (isActivated == true)
            {
                activationValue += Time.deltaTime * transitionSpeed;
                if (activationValue >= 1f)
                {
                    isTransitioning = false;
                }
                else
                {
                    float newScale = activationCurve.Evaluate(activationValue);
                    WeaponBody.gameObject.transform.localScale = new Vector3(1f, 1f, newScale);
                }
            }
            else
            {
                activationValue -= Time.deltaTime * transitionSpeed;
                if (activationValue <= 0f)
                {
                    isTransitioning = false;
                    WeaponBody.SetActive(false);
                }
                else
                {
                    float newScale = activationCurve.Evaluate(activationValue);
                    WeaponBody.gameObject.transform.localScale = new Vector3(1f, 1f, newScale);
                }
            }
        }
    }

    public void SetActiveMeleeWeapon(int index)
    {
        // Set all weapon data from array index into current weapon
        if (MeleeWeapons == null) { return; }
        if (index < 0 || index >= MeleeWeapons.Count) { return; }
        currentMelee = MeleeWeapons[index];
        WeaponBody = Instantiate(currentMelee.weaponData.WeaponPrefab, currentMelee.hand.transform.position, currentMelee.hand.transform.rotation);
        WeaponBody.transform.SetParent(currentMelee.hand.transform, true);
        WeaponBody.SetActive(false);
        Renderer renderer = WeaponBody.gameObject.GetComponentInChildren<Renderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    public void AttachToHand()
    {
        //currentMelee.body.transform.SetParent(currentMelee.hand.transform, false);
        if (isActivated == false)
        {
            WeaponBody.SetActive(true);
            activationValue = 0f;
            isActivated = true;
            isTransitioning = true;
        }
    }

    public void AttachToHolster()
    {
        //currentMelee.body.transform.SetParent(currentMelee.holster.transform, false);
        if (isActivated == true)
        {
            activationValue = 1f;
            isActivated = false;
            isTransitioning = true;
        }
    }

    public void IncrementAttackIndex()
    {
        if (attackIndex < currentMelee.weaponData.AttackAnimations.Count - 1)
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
        Collider[] collisions = Physics.OverlapCapsule(GetDamageCapsuleStart(), GetDamageCapsuleEnd(), currentMelee.weaponData.HitCapsuleRadius, damageLayer);
        foreach (Collider collision in collisions)
        {
            var damageable = collision.GetComponent<IHittable>();
            if (damageable == null) { return; }
            damageable.GetHit(currentMelee.weaponData.Damage, gameObject);
        }
    }

    private Vector3 GetDamageCapsuleStart()
    {
        return transform.position + currentMelee.weaponData.HitCapsuleForwardOffset * transform.forward;
    }

    private Vector3 GetDamageCapsuleEnd()
    {
        return transform.position + (currentMelee.weaponData.HitCapsuleForwardOffset + (currentMelee.weaponData.HitCapsuleHeight - 1f)) * transform.forward;
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos == false) { return; }
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GetDamageCapsuleStart(), currentMelee.weaponData.HitCapsuleRadius);
            Gizmos.DrawWireSphere(GetDamageCapsuleEnd(), currentMelee.weaponData.HitCapsuleRadius);
        }
    }
}
