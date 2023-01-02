using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomProjectile : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask damageLayer;

    // Projectile stats
    [Range(0f, 1f)] public float bounciness;
    [Range(1f, 100f)] public float speed = 1f;
    public bool useGravity;
    // Damage
    public int explosionDamage;
    public float explosionRange;
    // Lifetime
    public int maxCollisions = 1;
    public float maxLifetime = 0.5f;
    public bool explodeOnTouch = true;

    PhysicMaterial physicMat;
    public string targetTag;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Set up physicMat
        physicMat = new PhysicMaterial();
        physicMat.bounciness = bounciness;
        physicMat.frictionCombine = PhysicMaterialCombine.Minimum;
        physicMat.bounceCombine = PhysicMaterialCombine.Maximum;
        // Assign material to collider
        GetComponent<SphereCollider>().material = physicMat;

        // Set gravity
        rb.useGravity = useGravity;
        // Set speed
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    private void Update()
    {
        // Explode due to end of lifetime
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        // Instantiate explosion
        if (explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }
        // Instantiate sphere collider and deal damage
        Collider[] collisions = Physics.OverlapSphere(transform.position, explosionRange, damageLayer);
        foreach (Collider collision in collisions)
        {
            var damageable = collision.GetComponent<IHittable>();
            if (damageable != null)
            {
                damageable.GetHit(explosionDamage, gameObject);
            }
        }
        // Destroy after a small delay to avoid bugs
        Invoke("DelayDestroy", 0.025f);
    }

    private void DelayDestroy()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        // Don't explode if hitting other projectiles
        if (other.collider.CompareTag("Projectile")) { return; }

        // Explode if hits enemy directly
        if (explodeOnTouch == true)
        {
            Explode();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
