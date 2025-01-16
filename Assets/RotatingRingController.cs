using UnityEngine;
using Playniax.Pyro;
using Playniax.Ignition;
using System.Collections.Generic;

public class RotatingRingController : MonoBehaviour 
{
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float damagePerSecond = 50f;
    private float damageTimer;
    [SerializeField] private float damageInterval = 0.1f;
    
    [SerializeField] private float innerRadius = 1.5f; // Adjust in inspector to match inner ring radius
    [SerializeField] private float outerRadius = 2.5f; // Adjust in inspector to match outer ring radius

    // Keep track of when enemies were last damaged
    private static Dictionary<CollisionState, float> lastDamageTimes = new Dictionary<CollisionState, float>();

    void Awake()
    {
        // Ensure we have a collider
        var collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<PolygonCollider2D>();
        }
        collider.isTrigger = true;
        damageTimer = damageInterval;
    }

    void Update()
    {
        // Rotate the ring
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Check for collisions and apply damage
        damageTimer -= Time.deltaTime;
        if (damageTimer <= 0)
        {
            // Get all colliders in the outer circle
            var colliders = Physics2D.OverlapCircleAll(transform.position, outerRadius);
            foreach (var col in colliders)
            {
                // Skip if it's a player
                if (col.GetComponent<PlayerGroup>() != null) continue;

                var enemyCollisionState = col.GetComponent<CollisionState>();
                if (enemyCollisionState != null && enemyCollisionState.friend == null)
                {
                    // Calculate distance from ring center to enemy
                    float distanceToEnemy = Vector2.Distance(transform.position, col.transform.position);
                    
                    // Only damage if the enemy is between inner and outer radius
                    if (distanceToEnemy > innerRadius && distanceToEnemy < outerRadius)
                    {
                        // Check if enough time has passed since last damage
                        float lastDamageTime;
                        if (!lastDamageTimes.TryGetValue(enemyCollisionState, out lastDamageTime) || 
                            Time.time - lastDamageTime >= damageInterval)
                        {
                            float damageAmount = damagePerSecond * damageInterval;
                            enemyCollisionState.DoDamage(damageAmount);
                            enemyCollisionState.Ghost(); // Visual effect when taking damage
                            lastDamageTimes[enemyCollisionState] = Time.time;
                        }
                    }
                }
            }
            damageTimer = damageInterval;
        }
    }

    public void SetDamage(float damage)
    {
        damagePerSecond = damage;
    }

    void OnDestroy()
    {
        // Clear the static dictionary when the ring is destroyed
        lastDamageTimes.Clear();
    }

    // Visual debug to help set up the ring radii
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, innerRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, outerRadius);
    }
} 