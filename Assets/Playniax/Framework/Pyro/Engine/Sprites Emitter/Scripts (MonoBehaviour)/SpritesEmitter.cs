using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class SpritesEmitter : MonoBehaviour
    {
        [System.Serializable]
        public class Trails
        {
            [Tooltip("Determines if trails should be spawned.")]
            public bool spawn = true;

            [Tooltip("Position of the trails relative to the emitter.")]
            public Vector3 position;

            [Tooltip("Scale factor of the trails.")]
            public float scale = 1;

            [Tooltip("Initial scale of the trails.")]
            public Vector3 startScale;

            [Tooltip("Target scale of the trails.")]
            public Vector3 targetScale = new Vector3(5, .05f, 1);

            [Tooltip("Whether the trails should scale down over time.")]
            public bool scaleDownOvertime = true;

            [Tooltip("Sprite for the trails.")]
            public Sprite sprite;

            [Tooltip("Material for the trails.")]
            public Material material;

            [Tooltip("Time to live for each trail.")]
            public float ttl = 3;

            [Tooltip("Intensity of the trails.")]
            public int instensity = 1;

            [Tooltip("Order in layer for rendering.")]
            public int orderInLayer;

            [Tooltip("Start radius of the trails.")]
            public float startRadius = 10;

            [Tooltip("Target radius of the trails.")]
            public float targetRadius;

            [Tooltip("Start color of the trails.")]
            public Color startColor = new Color(1, 1, 1, 1);

            [Tooltip("Target color of the trails.")]
            public Color targetColor = new Color(1, 1, 1, 0);

            [Tooltip("Whether to randomize color within the given range.")]
            public bool randomColorRange = true;

            [Tooltip("Timer for spawning trails.")]
            public float timer;

            [Tooltip("Interval range for spawning trails.")]
            public FloatRange interval;

            [Tooltip("Time scale range for spawning trails.")]
            public FloatRange timeScale = new FloatRange(1);

            public void OnValidate()
            {
                if (instensity < 0) instensity = 0;
                if (ttl < 0) ttl = 0;

                if (interval.min < 0) interval.min = 0;
                if (interval.max < 0) interval.max = 0;

                if (timeScale.min < 0) timeScale.min = 0;
                if (timeScale.max < 0) timeScale.max = 0;
            }

            public void Spawn(Transform transform)
            {
                if (spawn == false) return;

                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    for (int i = 0; i < instensity; i++)
                        _CreateParticle(transform);

                    timer = interval.GetRandom();
                }
            }

            void _CreateParticle(Transform transform)
            {
                var spriteRenderer = new GameObject("Trails Particle").AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprite;
                spriteRenderer.material = material;

                var particle = spriteRenderer.gameObject.AddComponent<TrailsParticle>();
                particle.spriteRenderer = spriteRenderer;

                var angle = Random.Range(0, 360);
                var radians = angle * Mathf.Deg2Rad;

                var center = position;
                center += transform.position;

                //particle.transform.parent = transform;
                particle.transform.localRotation = Quaternion.Euler(0, 0, angle);

                var startPosition = center;
                startPosition.x += Mathf.Cos(radians) * startRadius;
                startPosition.y += Mathf.Sin(radians) * startRadius;

                var targetPosition = center;
                targetPosition.x += Mathf.Cos(radians) * targetRadius;
                targetPosition.y += Mathf.Sin(radians) * targetRadius;

                particle.startPosition = startPosition;
                particle.targetPosition = targetPosition;

                particle.scale = scale;

                particle.startScale = startScale;
                particle.targetScale = targetScale;

                particle.ttl = ttl;
                particle.timer = ttl;

                particle.timeScale = timeScale.GetRandom();

                if (randomColorRange)
                {
                    particle.startColor = _RandomColor(startColor, targetColor, startColor.a);
                    particle.targetColor = _RandomColor(startColor, targetColor, targetColor.a);
                }
                else
                {
                    particle.startColor = startColor;
                    particle.targetColor = targetColor;
                }

                particle.scaleDownOvertime = scaleDownOvertime;

                particle.Update();
            }

            Color _RandomColor(Color startColor, Color targetColor, float alpha)
            {
                float r = Random.Range(startColor.r, targetColor.r);
                float g = Random.Range(startColor.g, targetColor.g);
                float b = Random.Range(startColor.b, targetColor.b);

                return new Color(r, g, b, alpha);
            }
        }

        [Tooltip("Settings for the trails emitted by this object.")]
        public Trails trails = new Trails();

        void OnValidate()
        {
            trails.OnValidate();
        }

        void Update()
        {
            trails.Spawn(transform);
        }
    }
}
