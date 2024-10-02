using System.Collections.Generic;
using UnityEngine;

namespace Playniax.Pyro
{
    [DefaultExecutionOrder(500)]
    [AddComponentMenu("Playniax/Pyro/BulletBase")]
    public class BulletBase : MonoBehaviour
    {
        public Vector3 velocity;
        public bool isEnemyBullet = false; // Default to false (player's bullet)
        public static float enemyBulletSpeedMultiplier = 1.0f; // Add this line

        // Fields for bouncing bullets
        public bool isBouncingBullet = false; // Set to true for bouncing bullets
        public float bulletDamage = 1f; // Damage amount
        private HashSet<GameObject> enemiesHit; // Track enemies hit during current pass

        private Camera mainCamera;

        void Start()
        {
            if (isBouncingBullet)
            {
                mainCamera = Camera.main;
                enemiesHit = new HashSet<GameObject>();
            }
        }

        void Update()
        {
            float speedMultiplier = 1.0f;

            if (isEnemyBullet)
            {
                speedMultiplier = enemyBulletSpeedMultiplier;
            }

            transform.position += velocity * speedMultiplier * Time.deltaTime;

            if (isBouncingBullet)
            {
                BounceOffScreenEdges();
            }
        }

        void BounceOffScreenEdges()
        {
            if (mainCamera == null)
                return;

            Vector3 position = transform.position;
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(position);

            bool bounced = false;

            if (viewportPos.x <= 0f || viewportPos.x >= 1f)
            {
                velocity.x = -velocity.x;
                bounced = true;
            }

            if (viewportPos.y <= 0f || viewportPos.y >= 1f)
            {
                velocity.y = -velocity.y;
                bounced = true;
            }

            if (bounced)
            {
                // Reset the enemiesHit set when we bounce
                enemiesHit.Clear();

                // Clamp position to viewport bounds
                viewportPos.x = Mathf.Clamp01(viewportPos.x);
                viewportPos.y = Mathf.Clamp01(viewportPos.y);
                position = mainCamera.ViewportToWorldPoint(viewportPos);
                transform.position = position;
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var enemyScoreBase = other.GetComponent<IScoreBase>();
            if (enemyScoreBase != null && enemyScoreBase.friend != this.gameObject)
            {
                // Damage the enemy
                enemyScoreBase.structuralIntegrity -= bulletDamage;

                if (!isBouncingBullet)
                {
                    // Destroy the bullet only if it's not a bouncing bullet
                    Destroy(gameObject);
                }
                else
                {
                    // For bouncing bullets, ensure they hit an enemy only once per pass
                    if (!enemiesHit.Contains(other.gameObject))
                    {
                        enemiesHit.Add(other.gameObject);
                    }
                }
            }
        }
    }
}
