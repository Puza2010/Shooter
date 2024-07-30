using UnityEngine;

namespace Playniax.Pyro
{
    public class IntroEffect : MonoBehaviour
    {
        [System.Serializable]
        public class FlickerSettings
        {
            public int count = 15;
            public float sustain = 1f;
            public SpriteRenderer[] spriteRenderer = new SpriteRenderer[0];
            public CollisionBase2D[] collisionBase = new CollisionBase2D[0];
            public float counter { get; set; }
            public float timer { get; set; }
        }

        [System.Serializable]
        public class ScaleSettings
        {
            public float duration = 1;

            public Vector3 startScale = Vector3.zero;
            public Vector3 targetScale = Vector3.one;
            public bool inheritTargetScale;
            public bool destroy = true;

            public float timer { get; set; }
        }

        public enum Mode { Flicker, Scale };

        public Mode mode;
        public FlickerSettings flickerSettings = new FlickerSettings();
        public ScaleSettings scaleSettings = new ScaleSettings();

        void Start()
        {
            if (mode == Mode.Flicker)
            {
                if (CheckRenderer()) flickerSettings.spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
                if (CheckCollisionBase()) flickerSettings.collisionBase = GetComponentsInChildren<CollisionBase2D>();

                for (int i = 0; i < flickerSettings.collisionBase.Length; i++)
                    if (flickerSettings.collisionBase[i]) flickerSettings.collisionBase[i].suspended = true;
            }
            else if (mode == Mode.Scale)
            {
                if (scaleSettings.inheritTargetScale) scaleSettings.targetScale = transform.localScale;

                transform.localScale = scaleSettings.startScale;
            }

            bool CheckRenderer()
            {
                if (flickerSettings.spriteRenderer.Length == 0) return true;

                for (int i = 0; i < flickerSettings.spriteRenderer.Length; i++)
                    if (flickerSettings.spriteRenderer[i] == null) return true;

                return false;
            }

            bool CheckCollisionBase()
            {
                if (flickerSettings.collisionBase.Length == 0) return true;

                for (int i = 0; i < flickerSettings.collisionBase.Length; i++)
                    if (flickerSettings.collisionBase[i] == null) return true;

                return false;
            }
        }

        void Update()
        {
            if (mode == Mode.Flicker)
            {
                if (flickerSettings.collisionBase[0] && flickerSettings.collisionBase[0].suspended == false) return;

                if (flickerSettings.counter == flickerSettings.count)
                {
                    for (int i = 0; i < flickerSettings.collisionBase.Length; i++)
                        if (flickerSettings.collisionBase[i]) flickerSettings.collisionBase[i].suspended = false;
                }

                flickerSettings.timer += Time.deltaTime;

                if (flickerSettings.timer > (flickerSettings.sustain / 10))
                {
                    for (int i = 0; i < flickerSettings.spriteRenderer.Length; i++)
                        if (flickerSettings.spriteRenderer[i]) flickerSettings.spriteRenderer[i].enabled = !flickerSettings.spriteRenderer[i].enabled;

                    flickerSettings.counter += .5f;
                    flickerSettings.timer = 0;
                }
            }
            else if (mode == Mode.Scale)
            {
                transform.localScale = scaleSettings.targetScale - (scaleSettings.targetScale - scaleSettings.startScale) * ((scaleSettings.duration - scaleSettings.timer) / scaleSettings.duration);

                scaleSettings.timer += 1 * Time.deltaTime;

                if (scaleSettings.timer > scaleSettings.duration)
                {
                    transform.localScale = scaleSettings.targetScale;

                    if (scaleSettings.destroy && transform.localScale.magnitude == 0) Destroy(gameObject);
                }
            }
        }
    }
}
