using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace Playniax.Pyro
{
    public class DamageAccumulator : MonoBehaviour
    {
        [System.Serializable]
        public class AdditionalSettings
        {
            public UnityEvent onCollision;
            public UnityEvent onKill;
        }
        public bool useVelocity = true;
        public float velocityScale = 1;
        public CollisionState collisionState;
        public bool ignoreTilemapCollider;
        public string[] groupsToIgnore;
        public AdditionalSettings additionalSettings;

        void Awake()
        {
            if (collisionState == null) collisionState = GetComponent<CollisionState>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collisionState == null) return;
            if (isActiveAndEnabled == false) return;

            if (ignoreTilemapCollider && collision.gameObject.GetComponent<TilemapCollider2D>()) return;

            var enemyState = collision.gameObject.GetComponent<CollisionState>();
            if (enemyState && Ignore() == true) return;

            var damage = 1f; if (useVelocity) damage *= collision.relativeVelocity.magnitude * velocityScale;

            collisionState.structuralIntegrity -= damage;

            if (collisionState.structuralIntegrity <= 0)
            {
                collisionState.structuralIntegrity = 0;

                additionalSettings.onKill.Invoke();

                collisionState.Kill();
            }
            else
            {
                if (damage > 0)
                {
                    additionalSettings.onCollision.Invoke();

                    collisionState.Ghost();
                }
            }

            //if (collisionState.structuralIntegrity > 0 || enemy.structuralIntegrity > 0) CollisionAudio.Play(collisionState.material, enemy.material);

            bool Ignore()
            {
                for (int i = 0; i < groupsToIgnore.Length; i++)
                {
                    if (groupsToIgnore[i] == enemyState.group) return true;
                }
                return false;
            }
        }
    }
}