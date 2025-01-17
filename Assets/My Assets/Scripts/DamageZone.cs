using UnityEngine;
using Playniax.Pyro;
using Playniax.Ignition;
using System.Collections.Generic;

public class DamageZone : MonoBehaviour
{
    public float damage = 2f;
    public float damageInterval = 0.25f;
    private float damageTimer;
    
    // Keep track of when enemies were last damaged
    private static Dictionary<CollisionState, float> lastDamageTimes = new Dictionary<CollisionState, float>();

    private void Start()
    {
        damageTimer = damageInterval;
    }

    private void Update()
    {
        // Check for collisions and apply damage
        damageTimer -= Time.deltaTime;
        if (damageTimer <= 0)
        {
            var colliders = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius);
            foreach (var col in colliders)
            {
                if (col.GetComponent<PlayerGroup>() != null) continue;

                var enemyCollisionState = col.GetComponent<CollisionState>();
                if (enemyCollisionState != null && enemyCollisionState.friend == null)
                {
                    // Check if enough time has passed since last damage
                    float lastDamageTime;
                    if (!lastDamageTimes.TryGetValue(enemyCollisionState, out lastDamageTime) || 
                        Time.time - lastDamageTime >= damageInterval)
                    {
                        enemyCollisionState.DoDamage(damage);
                        enemyCollisionState.Ghost();
                        lastDamageTimes[enemyCollisionState] = Time.time;
                    }
                }
            }
            damageTimer = damageInterval;
        }
    }
} 