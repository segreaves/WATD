using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [field: SerializeField] private LayerMask damageLayer;
    [field: SerializeField] private List<Blade> Blades;
    [field: SerializeField] private List<Projectile> Projectiles;
    [field: SerializeField] public Blade currentBlade { get; private set; }
    [field: SerializeField] public AnimationCurve WeaponExtendCurve { get; private set; }
    [SerializeField] private GameObject Weapon;
    [SerializeField] private GameObject handObject;
    [SerializeField] private GameObject holsterObject;
    [SerializeField] private Animator Animator;
    public int attackIndex { get; private set; }
    public bool bladeEnabled { get; private set; }
    public bool aimEnabled { get; private set; }
    private float weaponExtendTimer;
    [field: SerializeField] private float transitionDuration = 0.2f;
    private Vector3 targetDimensions;
    private Vector3 dampingVelocity;
    [SerializeField] private bool showGizmos = true;

    private readonly int drawHash = Animator.StringToHash("ExtendCannon");
    private readonly int holsterHash = Animator.StringToHash("RetractCannon");

    private void Awake()
    {
        Animator = Weapon.GetComponent<Animator>();
    }

    void Start()
    {
        SetActiveBlade(0);
    }

    private void Update()
    {
        if (weaponExtendTimer > 0)
        {
            if (bladeEnabled)
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
        Holster();
    }

    public void ActivateBlade()
    {
        if (currentBlade == null) { return; }
        if (bladeEnabled == true) { return; }
        StartCoroutine(EActivateBlade());
    }

    public void DeactivateBlade()
    {
        if (currentBlade == null) { return; }
        if (bladeEnabled == false) { return; }
        StartCoroutine(EDeactivateBlade());
    }

    IEnumerator EActivateBlade()
    {
        bladeEnabled = true;
        currentBlade.model.transform.localScale = Vector3.zero;
        currentBlade.model.SetActive(true);
        weaponExtendTimer = transitionDuration;
        Draw();
        yield return new WaitForSeconds(transitionDuration);
        if (bladeEnabled == true)
        {
            currentBlade.model.transform.localScale = targetDimensions;
        }
    }

    IEnumerator EDeactivateBlade()
    {
        bladeEnabled = false;
        weaponExtendTimer = transitionDuration;
        yield return new WaitForSeconds(transitionDuration);
        if (aimEnabled == false)
            {
                Holster();
            }
        if (bladeEnabled == false)
        {
            currentBlade.model.SetActive(false);
            currentBlade.model.transform.localScale = Vector3.zero;
        }
    }

    public void ExtendCannon()
    {
        if (aimEnabled == true) { return; }
        StartCoroutine(EExtendCannon());
    }

    public void RetractCannon()
    {
        if (aimEnabled == false) { return; }
        StartCoroutine(ERetractCannon());
    }

    IEnumerator EExtendCannon()
    {
        aimEnabled = true;
        Draw();
        if (Animator != null)
        {
            Animator.CrossFadeInFixedTime(drawHash, 0.01f);
        }
        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator ERetractCannon()
    {
        aimEnabled = false;
        if (Animator != null)
        {
            Animator.CrossFadeInFixedTime(holsterHash, 0.01f);
        }
        yield return new WaitForSeconds(0.1f);
        Holster();
    }

    public void Draw()
    {
        Weapon.gameObject.transform.SetParent(handObject.transform, false);
    }

    public void Holster()
    {
        Weapon.gameObject.transform.SetParent(holsterObject.transform, false);
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
