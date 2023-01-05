using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomProjectile : MonoBehaviour
{
    public LayerMask damageLayer;
    [field: SerializeField] private LayerMask RayCastLayer;
    [field: SerializeField] private float ColliderRadius = 0.25f;
    private Rigidbody rb;
    public GameObject explosion;
    public float explosionScale = 1f;

    // Projectile stats
    [Range(0f, 1f)] public float bounciness;
    [Range(1f, 50f)] public float speed = 1f;
    public bool useGravity;
    public bool isAOE = false;
    // Damage
    public int explosionDamage;
    public float explosionRange;
    // Lifetime
    public float maxLifetime = 0.5f;
    public bool explodeOnTouch = true;

    private PhysicMaterial physicMat;
    private Vector3 prevPosition;

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
        // Set initial position
        prevPosition = transform.position;
    }

    private void Update()
    {
        // Explode due to end of lifetime
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0f)
        {
            Explode(transform.position);
        }
        // Check if there was a collision between frames
        RayTrace();
    }

    private void Explode(Vector3 explodePosition)
    {
        // Instantiate explosion
        if (explosion != null)
        {
            GameObject explosion_instance = Instantiate(explosion, explodePosition, Quaternion.identity);
            //explosion_instance.transform.SetParent(gameObject.transform);
            explosion_instance.transform.localScale = new Vector3(explosionScale, explosionScale, explosionScale);
            Destroy(explosion_instance, 1f);
        }
        // Instantiate sphere collider and deal damage
        Collider[] collisions = Physics.OverlapSphere(explodePosition, explosionRange, damageLayer);
        foreach (Collider collision in collisions)
        {
            var damageable = collision.GetComponent<IHittable>();
            if (damageable != null)
            {
                if (isAOE == true)
                {
                    Vector3 damageDirection = collision.gameObject.transform.position - explodePosition;
                    damageable.GetHit(explosionDamage, damageDirection);
                }
                else
                {
                    damageable.GetHit(explosionDamage, transform.forward);
                }
            }
        }
        // Destroy after a small delay to avoid bugs
        Invoke("DelayDestroy", 0.001f);
    }

    private void DelayDestroy()
    {
        Destroy(gameObject);
    }

    private void RayTrace()
    {
        // Set previous position for next iteration
        Vector3 prevPos = prevPosition;
        // Set current position as previous position for next iteration
        prevPosition = transform.position;
        // Raycast from previous position to current one
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.CapsuleCast(prevPos, transform.position, ColliderRadius, transform.forward, out hit, 1f, RayCastLayer))
        {
            //Debug.DrawLine(prevPos, hit.point, Color.yellow, 1f);
            Explode(hit.point);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
