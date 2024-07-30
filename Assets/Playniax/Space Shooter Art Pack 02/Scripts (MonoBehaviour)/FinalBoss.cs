using UnityEngine;
using Playniax.Pyro;
using Playniax.Ignition;
using Playniax.Portals;

namespace Playniax.SpaceShooterArtPack02
{
    public class FinalBoss : MonoBehaviour
    {

        [System.Serializable]
        public class GunSettings
        {
            [System.Serializable]
            public class BulletSettings
            {
                public GameObject prefab;
                public Vector3 position;
                public float speed = 16f;
                public Frames frames;
                public int count = 3;
                public float interval = .15f;
                public GameObject effectPrefab;
            }

            [System.Serializable]
            public struct Frames
            {
                public int left;
                public int right;
            }

            public Timer timer;
            public BulletSettings bulletSettings;
            public string animation;
            public int state;

            public bool Fire(int direction, Transform transform)
            {
                if (bulletSettings.prefab)
                {
                    if (_timer >= bulletSettings.interval)
                    {
                        _timer = 0;

                        Spawn();

                        _counter += 1;

                        if (_counter >= bulletSettings.count)
                        {
                            _counter = 0;

                            return true;
                        }
                    }
                    else
                    {
                        _timer += 1 * Time.deltaTime;

                        return false;
                    }
                }

                return false;

                void Spawn()
                {
                    var obj = Instantiate(bulletSettings.prefab, transform);
                    if (obj)
                    {
                        obj.transform.position = transform.position;
                        obj.transform.rotation = transform.rotation;

                        obj.transform.parent = null;

                        obj.SetActive(true);

                        var bulletBase = obj.GetComponent<BulletBase>();
                        if (bulletBase)
                        {
                            var position = bulletSettings.position;

                            position.x *= -direction;

                            bulletBase.velocity = new Vector3(bulletSettings.speed + Random.Range(0, 5), 0, 0) * direction;

                            bulletBase.transform.position += position;

                            if (direction == -1) bulletBase.transform.rotation *= Quaternion.Euler(0, 180, 0);

                            var spark = Instantiate(bulletSettings.effectPrefab);
                            spark.transform.position = bulletBase.transform.position;
                            spark.SetActive(true);
                        }
                    }
                }
            }

            int _counter;
            float _timer;
        }

        [System.Serializable]
        public class CutterSettings
        {
            public string animation;
        }

        [System.Serializable]
        public class GeneratorSettings
        {
            public Vector3 startPosition;
            public string animation;
        }

        public float timer = 3;
        public float interval = 3;
        public float intervalRange = 5;
        public float openSpeed = 1;
        public float maxDistance = 2;

        [Range(1, 100)]
        public int closingTreshold = 60;

        public SpriteState spriteState;

        public GunSettings topGunSettings;
        public GunSettings bottomGunSettings;

        public GeneratorSettings topGeneratorSettings;
        public GeneratorSettings bottomGeneratorSettings;

        public CutterSettings leftCutterSettings;
        public CutterSettings rightCutterSettings;

        public MonoBehaviour generate;

        void Start()
        {
            var topGenerator = spriteState.Find(topGeneratorSettings.animation);
            var bottomGenerator = spriteState.Find(bottomGeneratorSettings.animation);

            topGenerator.spriteRenderer.gameObject.transform.parent.localPosition = topGeneratorSettings.startPosition;
            bottomGenerator.spriteRenderer.gameObject.transform.parent.localPosition = bottomGeneratorSettings.startPosition;

            topGenerator.spriteRenderer.enabled = false;
            bottomGenerator.spriteRenderer.enabled = false;
        }

        bool _GetClosing()
        {
            var blackHole = (BlackHole)generate;
            if (blackHole) return blackHole.Closing;

            return false;
        }
        float _GetClosingState()
        {
            var blackHole = (BlackHole)generate;
            if (blackHole) return blackHole.ClosingState;

            return 100;
        }
        int _GetState()
        {
            var blackHole = (BlackHole)generate;
            if (blackHole) return blackHole.state;

            return -1;
        }

        void _SetState(int state)
        {
            var blackHole = (BlackHole)generate;
            if (blackHole) blackHole.state = state;
        }

        void Update()
        {

            if (_GetState() == -1) return;

            var topGenerator = spriteState.Find(topGeneratorSettings.animation);
            var bottomGenerator = spriteState.Find(bottomGeneratorSettings.animation);

            var cutterLeft = spriteState.Find(leftCutterSettings.animation);
            var cutterRight = spriteState.Find(rightCutterSettings.animation);

            _UpdateGun(topGunSettings.animation, topGunSettings);
            _UpdateGun(bottomGunSettings.animation, bottomGunSettings);

            if (_state == 2 || _state == 3)
            {
                cutterLeft.PlayOnce();
                cutterRight.PlayOnce();
            }

            if (_state == 0)
            {
                cutterLeft.PlayOnce(true);
                cutterRight.PlayOnce(true);
            }

            if (_state == 0)
            {
                timer -= 1 * Time.deltaTime;

                if (timer <= 0)
                {
                    timer = Random.Range(interval, interval + intervalRange);

                    topGenerator.spriteRenderer.enabled = true;
                    bottomGenerator.spriteRenderer.enabled = true;

                    _state = 1;
                }
            }

            if (_state == 1)
            {
                var topOnce = topGenerator.PlayOnce();
                var bottomOnce = bottomGenerator.PlayOnce();

                var distance = topGenerator.spriteRenderer.gameObject.transform.parent.localPosition.y - bottomGenerator.spriteRenderer.gameObject.transform.parent.localPosition.y;

                if (distance < maxDistance)
                {
                    topGenerator.spriteRenderer.gameObject.transform.parent.localPosition += Vector3.up * Time.deltaTime * openSpeed;
                    bottomGenerator.spriteRenderer.gameObject.transform.parent.localPosition += Vector3.down * Time.deltaTime * openSpeed;
                }

                if (bottomOnce == true) _SetState(1);

                //            Open black hole at certain distance in percentages:
                //            var percentage = 100 * distance / maxDistance;
                //            if (percentage > 25 && _GetState() == 0) _SetState(1);

                //            Open black hole at have of the distance:
                //            if (distance >= maxDistance / 2 && _GetState() == 0) _SetState(1);

                if (distance >= maxDistance && topOnce == true && bottomOnce == true) _state = 2;
            }

            if (_state == 2)
            {
                if (_GetClosing() && _GetClosingState() < closingTreshold) _state = 3;
            }

            if (_state == 3)
            {
                var topOnce = topGenerator.PlayOnce(true);
                var bottomOnce = bottomGenerator.PlayOnce(true);

                topGenerator.spriteRenderer.gameObject.transform.parent.localPosition += Vector3.down * Time.deltaTime * openSpeed;
                bottomGenerator.spriteRenderer.gameObject.transform.parent.localPosition += Vector3.up * Time.deltaTime * openSpeed;

                var distance = topGenerator.spriteRenderer.gameObject.transform.parent.localPosition.y - bottomGenerator.spriteRenderer.gameObject.transform.parent.localPosition.y;

                var stop = topGeneratorSettings.startPosition - bottomGeneratorSettings.startPosition;

                if (topOnce == true && bottomOnce == true)
                {
                    topGenerator.spriteRenderer.enabled = false;
                    bottomGenerator.spriteRenderer.enabled = false;
                }

                if (distance <= stop.y)
                {
                    topGenerator.spriteRenderer.gameObject.transform.parent.localPosition = topGeneratorSettings.startPosition;
                    bottomGenerator.spriteRenderer.gameObject.transform.parent.localPosition = bottomGeneratorSettings.startPosition;

                    _state = 0;
                }
            }
        }

        void _UpdateGun(string animation, GunSettings settings)
        {
            if (settings.state == 0)
            {
                if (settings.timer.Update())
                {
                    settings.state = 1;
                }
            }

            // Fire left
            if (settings.state == 1)
            {
                var gun = spriteState.Find(animation);

                var frame = gun.GetFrame();

                if (frame == settings.bulletSettings.frames.left)
                {
                    var fire = settings.Fire(-1, gun.spriteRenderer.transform);
                    if (fire) settings.state = 2;
                }
                else
                {
                    gun.PlayOnce();
                }
            }

            // Fire right
            if (settings.state == 2)
            {
                var gun = spriteState.Find(animation);

                var frame = gun.GetFrame();

                if (frame == settings.bulletSettings.frames.right)
                {
                    var fire = settings.Fire(1, gun.spriteRenderer.transform);
                    if (fire) settings.state = 3;
                }
                else
                {
                    gun.PlayOnce();
                }
            }

            // Retract
            if (settings.state == 3)
            {
                var gun = spriteState.Find(animation);

                var play = gun.PlayOnce();

                if (play)
                {
                    gun.Reset();

                    settings.state = 0;
                }
            }
        }

        int _state;
    }
}