using UnityEngine;
using Playniax.Ignition;
using Playniax.Pyro;

namespace Playniax.Portals
{
    public class BlackHole : IgnitionBehaviour
    {
        public class Glow : MonoBehaviour
        {
            public float position;
            public float speed;
            public SpriteRenderer spriteRenderer;
            public BlackHole portal;

            void Update()
            {
                spriteRenderer.sortingOrder = portal.spriteRenderer.sortingOrder + Random.Range(-1, 1);

                var x = Mathf.Cos(position) * portal.glowPosition;
                var y = Mathf.Sin(position) * portal.glowPosition;

                transform.localPosition = new Vector3(x, y);

                position += speed * Time.deltaTime;
            }
        }

        [System.Serializable]
        public class SoundSettings
        {
            public AudioProperties open;
            public AudioProperties outro;
            public AudioProperties close;
        }

        [System.Serializable]
        public class Targets
        {
            public bool player = true;
            public bool enemy;
        }

        public int state = -1;
        public float growSize = 1;
        public int growSteps = 50;
        public float sustain = 3;
        public float range = 10;
        public float force = 1;
        public Sprite glowSprite;
        public float glowPosition = 1.11f;
        public float glowSize = .5f;
        public int glowParticles = 25;
        public float glowRotationSpeed = 1;
        public Material glowMaterial;
        public GameObject effectPrefab;
        public SpritesEmitter.Trails trails;
        public int orderInLayer;
        public Targets targets;
        public SpriteRenderer spriteRenderer;
        public SoundSettings soundSettings;
        //public string[] target = new string[] { "Enemy", "Player" };

        public bool Closing
        {
            get
            {
                if (state == 4) return true;

                return false;
            }
        }

        public float ClosingState
        {
            get
            {
                var scale = Mathf.Min(transform.localScale.x, transform.localScale.y);

                scale = Mathf.Abs(scale);

                if (scale < 0) scale = 0;
                if (scale > growSize) scale = growSize;

                var closingState = 100 * scale / growSize;

                return closingState;
            }
        }

        public override void OnInitialize()
        {
            if (effectPrefab && effectPrefab.scene.rootCount > 0) effectPrefab.SetActive(false);

        }
        public override void Awake()
        {
            base.Awake();

            RendererHelpers.SetOrder(gameObject, orderInLayer, true);

            _InitGlow();

            transform.localScale = new Vector3(0, 0, 1);

            GameObjectHelpers.SetActiveChildren(gameObject, false);

            state = 0;
        }

        void LateUpdate()
        {
            _Update();
        }

        void _InitGlow()
        {
            for (int i = 0; i < glowParticles; i++)
            {
                var glow = new GameObject("Glow " + (i + 1)).AddComponent<Glow>();
                glow.portal = this;
                glow.speed = Random.Range(-glowRotationSpeed, glowRotationSpeed);
                glow.position = Random.Range(0, 359) * Mathf.Deg2Rad;
                glow.transform.localScale *= Random.Range(glowSize / 2, glowSize);

                glow.spriteRenderer = glow.gameObject.AddComponent<SpriteRenderer>();
                glow.spriteRenderer.sprite = glowSprite;
                glow.spriteRenderer.material = glowMaterial;
                glow.spriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;

                glow.transform.SetParent(spriteRenderer.transform);
            }
        }

        void _Update()
        {
            if (state == 1)
            {
                GameObjectHelpers.SetActiveChildren(gameObject, true);

                soundSettings.open.Play();

                state = 2;
            }
            else if (state == 2)
            {
                _ParticleEffects();

                transform.localScale += new Vector3(growSize / growSteps, growSize / growSteps, 0) * Time.deltaTime * 50;

                if (transform.localScale.x >= growSize)
                {
                    state = 3;
                }
            }
            else if (state == 3)
            {
                _ParticleEffects();

                _Pull();

                _sustain += Time.deltaTime;

                if (_sustain >= sustain)
                {
                    _sustain = 0;

                    state = 4;
                }
            }
            else if (state == 4)
            {
                transform.localScale -= new Vector3(growSize / growSteps, growSize / growSteps, 0) * Time.deltaTime * 50;

                if (transform.localScale.x <= 0)
                {
                    GameObjectHelpers.SetActiveChildren(gameObject, false);

                    state = 0;

                    soundSettings.close.Play();
                }
            }
        }

        void _ParticleEffects()
        {
            trails.Spawn(transform);
        }

        void _Pull()
        {
            GameObject[] objects = new GameObject[0];

            if (targets.player)
            {
                var players = FindObjectsOfType<PlayerGroup>();
                objects = new GameObject[players.Length];
                for (int i = 0; i < objects.Length; i++)
                    objects[i] = players[i].gameObject;
                Pull();
            }

            if (targets.enemy)
            {
                var enemies = FindObjectsOfType<EnemyAI>();
                objects = new GameObject[enemies.Length];
                for (int i = 0; i < objects.Length; i++)
                    objects[i] = enemies[i].gameObject;
                Pull();
            }

            void Pull()
            {
                var targetPosition = growSize * Random.Range(.25f, .75f);

                for (int i = 0; i < objects.Length; i++)
                {
                    var target = objects[i];

                    var angle = Math2DHelpers.GetAngle(transform.position, target.transform.position);

                    var velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));

                    var distance = Vector3.Distance(target.transform.position, transform.position);

                    velocity *= (int)(range / distance);

                    velocity *= force;

                    target.transform.position += velocity * Time.deltaTime;

                    distance = Vector3.Distance(target.transform.position, transform.position);

                    if (distance < targetPosition)
                    {
                        var effect = Instantiate(effectPrefab, target.transform.position, Quaternion.identity);

                        effect.SetActive(true);

                        Destroy(target.gameObject);

                        soundSettings.outro.Play();
                    }
                }
            }
        }

        float _sustain;
    }
}
