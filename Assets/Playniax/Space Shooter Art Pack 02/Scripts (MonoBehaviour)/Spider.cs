using UnityEngine;
using Playniax.Pyro;

namespace Playniax.SpaceShooterArtPack02
{
    public class Spider : AnimationPool
    {
        public BulletSpawnerBase bulletSpawner;
        public SpriteRenderer spriteRenderer;
        void Awake()
        {
            if (bulletSpawner == null) bulletSpawner = GetComponent<BulletSpawnerBase>();

            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (_state == 0)
            {
                var idle = Find("Idle");

                idle.Play(spriteRenderer, .05f);

                var velocity = gameObject.transform.position - _position;

                if (velocity.magnitude < .005f) _state += 1;

                _position = transform.position;

                //if ((startPosition == StartPosition.Left || startPosition == StartPosition.Right) && Mathf.Abs(_velocity.x) < .25f && idle.isLastFrame) _state += 1;
                //if ((startPosition == StartPosition.Top || startPosition == StartPosition.Bottom) && Mathf.Abs(_velocity.y) < .25f && idle.isLastFrame) _state += 1;
            }
            else if (_state == 1)
            {
                if (Find("Tilt").PlayOnce(spriteRenderer, .05f) == true) _state++;
            }
            else if (_state == 2)
            {
                Find("Spinning").Play(spriteRenderer, .05f);

                if (bulletSpawner && bulletSpawner.automatically == false) bulletSpawner.UpdateSpawner();
            }
        }

        Vector3 _position;
        int _state;
    }
}
