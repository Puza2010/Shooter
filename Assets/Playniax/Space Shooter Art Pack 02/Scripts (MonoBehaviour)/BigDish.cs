using UnityEngine;
using Playniax.Ignition;
using Playniax.Pyro;

namespace Playniax.SpaceShooterArtPack02
{
    public class BigDish : IgnitionBehaviour
    {
        [System.Serializable]
        public class FireballSettings
        {
            public GameObject prefab;
            public Timer timer = new Timer();
            public float speed = 4;
            public float speedRange = 0;
            public float position = .75f;
            public float scale = 1;
            public int step = 15;
        }

        [System.Serializable]
        public class PowerballSettings
        {
            public GameObject prefab;
            public Timer timer = new Timer();
            public float speed = 8;
            public float speedRange = 8;
            public Vector3 position;
            public float scale = 1;
        }

        public int state;
        public float timer = 8;
        public SpriteState spriteState;

        public FireballSettings fireballSettings = new FireballSettings();
        public PowerballSettings powerballSettings = new PowerballSettings();

        public override void OnInitialize()
        {
            if (fireballSettings.prefab && fireballSettings.prefab.scene.rootCount > 0) fireballSettings.prefab.SetActive(false);
            if (powerballSettings.prefab && powerballSettings.prefab.scene.rootCount > 0) powerballSettings.prefab.SetActive(false);
        }
        void Update()
        {
            if (state == 0)
            {
                _PowerBalls();

                var idle = spriteState.Find("Idle");

                var cycle = idle.Play();

                timer -= 1 * Time.deltaTime;

                if (cycle && timer <= 0)
                {
                    state = 1;
                }
            }
            else if (state == 1)
            {
                var tilt = spriteState.Find("Tilt");

                if (tilt.PlayOnce() == true)
                {
                    timer = Random.Range(3, 6);

                    tilt.Reset();

                    state = 2;
                }
            }
            else if (state == 2)
            {
                _FireBalls();

                var spinning = spriteState.Find("Spinning");

                var cycle = spinning.Play();

                timer -= 1 * Time.deltaTime;

                if (cycle && timer <= 0)
                {
                    state = 3;
                }
            }
            else if (state == 3)
            {
                var normalize = spriteState.Find("Normalize");

                if (normalize.PlayOnce() == true)
                {
                    timer = Random.Range(3, 6);

                    normalize.Reset();

                    state = 0;
                }
            }
        }

        void _FireBalls()
        {
            if (fireballSettings.timer.Update() == false) return;

            var instance = Instantiate(fireballSettings.prefab, transform.position, transform.rotation);
            if (instance == null) return;

            var introEffect = instance.GetComponent<IntroEffect>();
            if (introEffect == null) introEffect = instance.AddComponent<IntroEffect>();
            introEffect.mode = IntroEffect.Mode.Scale;
            introEffect.scaleSettings.duration = .5f;

            var bulletBase = instance.GetComponent<BulletBase>();
            if (bulletBase == null) bulletBase = instance.AddComponent<BulletBase>();

            instance.transform.eulerAngles = new Vector3(0, 0, _fireballAngle * Mathf.Rad2Deg);

            instance.transform.Translate(Vector3.right * fireballSettings.position);

            instance.transform.localScale *= fireballSettings.scale;

            bulletBase.velocity = new Vector3(Mathf.Cos(_fireballAngle), Mathf.Sin(_fireballAngle)) * Random.Range(fireballSettings.speed, fireballSettings.speed + fireballSettings.speedRange);

            instance.SetActive(true);

            _fireballAngle += fireballSettings.step * Time.deltaTime;
        }
        void _PowerBalls()
        {
            if (powerballSettings.timer.Update() == false) return;

            var instance = Instantiate(powerballSettings.prefab, transform.position, transform.rotation);
            if (instance == null) return;

            var introEffect = instance.GetComponent<IntroEffect>();
            if (introEffect == null) introEffect = instance.AddComponent<IntroEffect>();
            introEffect.mode = IntroEffect.Mode.Scale;
            introEffect.scaleSettings.duration = .5f;

            var bulletBase = instance.GetComponent<BulletBase>();
            if (bulletBase == null) bulletBase = instance.AddComponent<BulletBase>();

            instance.transform.Translate(powerballSettings.position);

            instance.transform.localScale *= powerballSettings.scale;

            var target = PlayerGroup.GetRandom();

            if (target)
            {
                var angle = Mathf.Atan2(target.transform.position.y - instance.transform.position.y, target.transform.position.x - instance.transform.position.x);

                bulletBase.velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(powerballSettings.speed, powerballSettings.speed + powerballSettings.speedRange);
            }
            else
            {
                var angle = Random.Range(0, 359) * Mathf.Deg2Rad;

                bulletBase.velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(powerballSettings.speed, powerballSettings.speed + powerballSettings.speedRange);
            }

            instance.SetActive(true);
        }

        float _fireballAngle;
    }
}