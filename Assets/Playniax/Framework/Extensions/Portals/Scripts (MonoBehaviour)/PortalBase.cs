using UnityEngine;
using Playniax.Ignition;
using Playniax.Sequencer;

namespace Playniax.Portals
{
    public class PortalBase : AdvancedSpawnerBase
    {
        public class Glow : MonoBehaviour
        {
            public float position;
            public float speed;
            public SpriteRenderer spriteRenderer;
            public PortalBase portal;

            void Update()
            {
                spriteRenderer.sortingOrder = portal.particleSettings.spriteRenderer.sortingOrder + Random.Range(-1, 1);

                var x = Mathf.Cos(position) * portal.particleSettings.glowPosition;
                var y = Mathf.Sin(position) * portal.particleSettings.glowPosition;

                transform.localPosition = new Vector3(x, y);

                position += speed * Time.deltaTime;
            }
        }

        public class Intro : MonoBehaviour
        {
            public Vector3 scale;
            public int steps = 10;
            void Update()
            {
                var localScale = transform.localScale;

                localScale.x += (scale.x - localScale.x) / steps;
                localScale.y += (scale.y - localScale.y) / steps;
                localScale.z += (scale.y - localScale.z) / steps;

                transform.localScale = localScale;

                if (transform.localScale == scale) Destroy(this);
            }
        }

        [System.Serializable]
        public class ParticleSettings
        {
            public Sprite glowSprite;
            public float glowPosition = 1.11f;
            public float glowSize = .5f;
            public int glowParticles = 25;
            public float glowRotationSpeed = 1;
            public Material glowMaterial;
            public int growSteps = 50;
            public float growScale = 1;
            public SpriteRenderer spriteRenderer;
        }

        [System.Serializable]
        public class SoundSettings
        {
            public AudioProperties open = new AudioProperties();
            public AudioProperties intro = new AudioProperties();
            public AudioProperties close = new AudioProperties();
        }

        public bool randomPrefab;
        public bool autoPopulateParent = true;
        public Transform parent;
        public float timer;
        public float interval;
        public float intervalRange;
        public int counter = 1;
        public int maxAtOnce = 1;
        public int introSteps = 10;

        public ParticleSettings particleSettings = new ParticleSettings();
        public SoundSettings soundSettings = new SoundSettings();
        public float growSize
        {
            get { return _growSize * particleSettings.growScale; }
        }

        public virtual bool isAllowed
        {
            get { return true; }
        }

        public override void OnInitialize()
        {
            for (int i = 0; i < prefabs.Length; i++)
                if (prefabs[i] && prefabs[i].scene.rootCount > 0) prefabs[i].SetActive(false);
        }

        public virtual bool SetPosition()
        {
            return true;
        }

        public override void OnSequencerAwake()
        {
            _InitGlow();
            _InitSpawner();
        }

        public override void OnSequencerUpdate()
        {
            _Update();
        }

        public virtual GameObject OnSpawn()
        {
            if (autoPopulateParent == true && parent == null) parent = transform.parent;

            var instance = Instantiate(prefabs[_index], transform.position, Quaternion.identity, parent);

            instance.AddComponent<Register>();
            instance.AddComponent<ProgressCounter>();

            var intro = instance.AddComponent<Intro>();
            intro.steps = introSteps;
            intro.scale = instance.transform.localScale;

            instance.transform.localScale = Vector3.zero;

            soundSettings.intro.Play();

            instance.SetActive(true);

            return instance;
        }

        void _GetPrefab()
        {
            if (randomPrefab)
            {
                _index = Random.Range(0, prefabs.Length);
            }
            else
            {
                _index += 1; if (_index >= prefabs.Length) _index = 0;
            }

            var size = RendererHelpers.GetSize(prefabs[_index]);

            _growSize = Mathf.Max(size.x, size.y) * .5f;
        }

        void _InitGlow()
        {
            for (int i = 0; i < particleSettings.glowParticles; i++)
            {
                var glow = new GameObject("Glow " + (i + 1)).AddComponent<Glow>();
                glow.portal = this;
                glow.speed = Random.Range(-particleSettings.glowRotationSpeed, particleSettings.glowRotationSpeed);
                glow.position = Random.Range(0, 359) * Mathf.Deg2Rad;
                glow.transform.localScale *= Random.Range(particleSettings.glowSize / 2, particleSettings.glowSize);

                glow.spriteRenderer = glow.gameObject.AddComponent<SpriteRenderer>();
                glow.spriteRenderer.sprite = particleSettings.glowSprite;
                glow.spriteRenderer.material = particleSettings.glowMaterial;
                glow.spriteRenderer.sortingOrder = particleSettings.spriteRenderer.sortingOrder - 1;

                glow.transform.SetParent(particleSettings.spriteRenderer.transform);
            }
        }
        void _InitSpawner()
        {
            if (prefabs.Length > 0)
            {
                transform.localScale = new Vector3(0, 0, 1);

                particleSettings.spriteRenderer.gameObject.SetActive(false);
            }

            ProgressCounter.Add(counter);

            /*
            if (spawnerSettings.trackProgress)
            {
                GameData.progressScale += spawnerSettings.counter;
                GameData.progress += spawnerSettings.counter;
            }
            */
        }

        void _Update()
        {
            if (prefabs.Length == 0) return;

            if (isAllowed && _state == 0)
            {
                if (counter == -1)
                {
                    if (timer <= 0)
                    {
                        _GetPrefab();

                        _state = 1;

                        timer = Random.Range(interval, interval + intervalRange);
                    }
                    else
                    {
                        timer -= 1 * Time.deltaTime;
                    }
                }
                else if (counter > 0)
                {
                    if (timer <= 0)
                    {
                        _GetPrefab();

                        _state = 1;

                        counter -= 1;

                        if (counter > 0)
                        {
                            timer = Random.Range(interval, interval + intervalRange);
                        }
                    }
                    else
                    {
                        timer -= 1 * Time.deltaTime;
                    }
                }

            }
            else if (_state == 1)
            {
                if (SetPosition() == true)
                {
                    particleSettings.spriteRenderer.gameObject.SetActive(true);

                    soundSettings.open.Play();

                    _state = 2;
                }
            }
            else if (_state == 2)
            {
                var size = _growSize * particleSettings.growScale;

                transform.localScale += new Vector3(size / particleSettings.growSteps, size / particleSettings.growSteps, 0) * Time.deltaTime * 50;

                if (transform.localScale.x >= size)
                {
                    _space = 1;

                    _state = 3;
                }
            }
            else if (_state == 3)
            {
                _space -= Time.deltaTime;

                if (_space <= 0)
                {
                    if (maxAtOnce <= 1)
                    {
                        _state = 4;
                    }
                    else
                    {
                        if (counter == -1)
                        {
                            _multipleCounter = Random.Range(0, maxAtOnce);
                        }
                        else
                        {
                            _multipleCounter = Random.Range(0, maxAtOnce);

                            if (_multipleCounter > counter) _multipleCounter = counter;

                            counter -= _multipleCounter;
                        }

                        _state = 5;
                    }
                }
            }
            else if (_state == 4)
            {
                OnSpawn();

                _space = 1;

                _state = 6;
            }
            else if (_state == 5)
            {
                _space -= Time.deltaTime;

                if (_space <= 0)
                {
                    OnSpawn();

                    _multipleCounter -= 1;

                    if (_multipleCounter < 0)
                    {
                        _space = 1;

                        _state = 6;
                    }
                    else
                    {
                        _space = .5f;
                    }
                }
            }
            else if (_state == 6)
            {
                _space -= Time.deltaTime;

                if (_space <= 0)
                {
                    _state = 7;
                }

            }
            else if (_state == 7)
            {
                var size = _growSize * particleSettings.growScale;

                transform.localScale -= new Vector3(size / particleSettings.growSteps, size / particleSettings.growSteps, 0) * Time.deltaTime * 50;

                if (transform.localScale.x <= 0)
                {
                    if (counter == 0)
                    {
                        enabled = false;
                    }
                    else
                    {
                        particleSettings.spriteRenderer.gameObject.SetActive(false);

                        _state = 0;
                    }

                    soundSettings.close.Play();
                }
            }
        }

        float _growSize;
        int _index = -1;
        int _multipleCounter;
        float _space;
        int _state;
    }
}