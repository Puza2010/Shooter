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

        void Update()
        {
            float speedMultiplier = 1.0f;

            if (isEnemyBullet)
            {
                speedMultiplier = enemyBulletSpeedMultiplier;
            }

            transform.position += velocity * speedMultiplier * Time.deltaTime;
        }
    }

}