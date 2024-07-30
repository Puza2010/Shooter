using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class EnergyBeam : CollisionBase2D
    {
        public int playerIndex = -1;
        public int damage = 1;
        public float ttl = 60;

        public override void OnCollision(CollisionBase2D collision)
        {
            _UpdateState(collision);
        }

        public override void Awake()
        {
            base.Awake();

            _spriteRendererGroup = gameObject.GetComponent<SpriteRendererGroup>();
        }

        void Update()
        {
            ttl -= 1 * Time.deltaTime;

            if (ttl < 0)
            {
                if (_spriteRendererGroup)
                {
                    _spriteRendererGroup.alpha -= 1 * Time.deltaTime;

                    if (_spriteRendererGroup.alpha <= 0) Destroy(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else if (_spriteRendererGroup)
            {
                if (_spriteRendererGroup.alpha < 1)
                {
                    _spriteRendererGroup.alpha += 1 * Time.deltaTime;
                }
            }
        }

        void _UpdateState(CollisionBase2D collision)
        {
            var collisionState = collision as CollisionState;

            if (collisionState == null) return;

            collisionState.DoDamage(damage);

            if (collisionState.structuralIntegrity > 0) collisionState.Ghost();

            if (playerIndex > -1 && collisionState.structuralIntegrity == 0)
            {
                PlayerData.Get(playerIndex).scoreboard += collisionState.points;

                CollisionState.OutroSettings.MessengerSettings.Message(collisionState);
            }
        }

        SpriteRendererGroup _spriteRendererGroup;
    }
}