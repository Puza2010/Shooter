using UnityEngine;
using Playniax.Ignition;
using Playniax.Pyro;

namespace Playniax.SpaceShooterArtPack02
{
    public class Tank : AnimationPool
    {
        [System.Serializable]
        public class BulletProperties
        {
            public GameObject prefab;
            public float timer = 1;
            public float interval = .25f;
            public float speed = 8;

            public Color glowColor;
            public SpriteRenderer glow;

            //public TilemapCollider2D[] tilemapColliders;
        }

        public BulletProperties bulletProperties;

        public SpriteRenderer turret;
        public SpriteRenderer wheels;

        public int turretFrames;

        public bool limitRange;
        public float range = 8;

        void Update()
        {
            if (turret == null) return;
            if (wheels == null) return;

            _UpdateDriving();
            _UpdateTurret();
            _UpdateGlow();
        }

        void _UpdateDriving()
        {
            if (_state == -1)
            {
                _start = transform.position;

                bulletProperties.glowColor = bulletProperties.glow.color;
                bulletProperties.glow.enabled = false;

                if (Find("Turret") != null) turretFrames = Find("Turret").sprites.Length;

                _state = 0;
            }
            else if (_state == 0)
            {
                _speed += new Vector3(Random.Range(-4, 4), 0, 0) * 60 * Time.deltaTime;

                _timer = Random.Range(1, 2);

                _state = 1;
            }
            else if (_state == 1)
            {
                _timer -= 1 * Time.deltaTime;

                if (_timer < 0) _state = 0;
            }

            transform.localPosition += _speed * Time.deltaTime;

            if (limitRange && Mathf.Abs(transform.localPosition.x - _start.x) >= range)
            {
                _speed.x = -_speed.x;

                transform.localPosition += _speed * Time.deltaTime;

                _timer = Random.Range(1, 2);
            }
            else
            {
                _speed *= 1 / (1 + (Time.deltaTime * .5f));
            }

            //if (Mathf.Abs(_speed.x) >= 1) EmitterGroup.Play("Smoke", transform.position + Vector3.down * .5f, 1, -2);

            if (_speed.x < 0)
            {
                Find("Wheels").Play(wheels, _speed.x, true);
            }
            if (_speed.x > 0)
            {
                Find("Wheels").Play(wheels, _speed.x);
            }
        }

        void _UpdateTurret()
        {
            if (_state < 0) return;

            _target = PlayerGroup.GetRandom(_target);
            
            if (_target == null) return;

            var angle = Mathf.Atan2(_target.transform.position.y - transform.position.y, _target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;

            if (angle < 0) angle += 360;

            if (angle < 0) return;
            if (angle > 179) return;

            float frame = angle / 5;

            if (frame < 0) frame = 0;
            if (frame > turretFrames - 1) frame = turretFrames - 1;

            turret.sprite = Find("Turret").GetSprite(Mathf.RoundToInt(frame));

            //bulletProperties.Update(transform.position + _bulletPositions[Mathf.RoundToInt(frame)] / 100f, _target);

            Fire();

            void Fire()
            {
                var position = transform.position + _bulletPositions[Mathf.RoundToInt(frame)] / 100f;

                bulletProperties.glow.transform.position = position;

                bulletProperties.timer -= 1 * Time.deltaTime;
                if (bulletProperties.timer > 0) return;

                bulletProperties.timer = bulletProperties.interval;

                var instance = Instantiate(bulletProperties.prefab, position, Quaternion.identity, transform.parent);
                if (instance == null) return;

                var bullet = instance.GetComponent<BulletBase>();
                if (bullet == null) return;

                instance.SetActive(true);

                var angle = Mathf.Atan2(_target.transform.position.y - position.y, _target.transform.position.x - position.x);

                bullet.velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * bulletProperties.speed;

                bulletProperties.glow.color = bulletProperties.glowColor;
                bulletProperties.glow.enabled = true;
/*
                if (bulletProperties.tilemapCollider != null)
                {
                    var collisionState = instance.GetComponent<CollisionState>();
                    if (collisionState)
                    {
                        var tileCollider = instance.GetComponent<TileCollider2D>();
                        if (tileCollider == null) tileCollider = instance.AddComponent<TileCollider2D>();
                        tileCollider.tilemapCollider = bulletProperties.tilemapCollider;
                        tileCollider.onCollision.AddListener(delegate { collisionState.Kill(); });
                    }
                }
*/
            }
        }

        void _UpdateGlow()
        {
            if (_state < 0) return;

            if (bulletProperties.glow.enabled == false) return;

            bulletProperties.glow.color -= new Color(0, 0, 0, 05f) * Time.deltaTime;

            if (bulletProperties.glow.color.a <= 0) bulletProperties.glow.enabled = false;
        }

        int _state = -1;
        Vector3 _speed;
        GameObject _target;
        float _timer;
        Vector3 _start;

        Vector3[] _bulletPositions = new[]
        {
            new Vector3(44, -2),
            new Vector3(43, 0),
            new Vector3(41, 3),
            new Vector3(39, 6),
            new Vector3(36, 8),
            new Vector3(33, 10),
            new Vector3(30, 12),
            new Vector3(27, 13),
            new Vector3(24, 14),
            new Vector3(21, 15),
            new Vector3(18, 16),
            new Vector3(15, 17),
            new Vector3(13, 18),
            new Vector3(10, 19),
            new Vector3(7, 20),
            new Vector3(5, 21),
            new Vector3(2, 22),
            new Vector3(0, 23),
            new Vector3(-3, 21),
            new Vector3(-5, 20),
            new Vector3(-8, 18),
            new Vector3(-10, 17),
            new Vector3(-13, 17),
            new Vector3(-15, 16),
            new Vector3(-18, 15),
            new Vector3(-20, 14),
            new Vector3(-23, 13),
            new Vector3(-26, 12),
            new Vector3(-29, 11),
            new Vector3(-32, 10),
            new Vector3(-35, 8),
            new Vector3(-38, 6),
            new Vector3(-41, 3),
            new Vector3(-43, 0),
            new Vector3(-44, -2),
            new Vector3(-45, -5)
        };
    }
}