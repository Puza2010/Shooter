#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using Playniax.Ignition;
using Playniax.Pyro;

namespace Playniax.SpaceShooterArtPack02
{
    public class Mine : AnimationPool
    {
        [System.Serializable]
        public class OutroSettings
        {
            [System.Serializable]
            // On outro effects settings.
            public class EffectsSettings
            {
                // GameObject to instantiate.
                public GameObject prefab;
                // Scale of the GameObject.
                public float scale = 1;
                // Sorting order.
                public int orderInLayer = 0;
                // Reference size.
                public float sizeInPixels = 80;
            }

            public EffectsSettings[] effectsSettings;
            public AudioProperties sound;
        }

        [System.Serializable]
        public class ShrapnelSettings
        {
            public GameObject prefab;
            public float scale = 1;
            public float speed = 12;
            public int parts = 16;
        }

        public float armedDistance = 3f;
        public OutroSettings outroSettings = new OutroSettings();
        public ShrapnelSettings shrapnelSettings = new ShrapnelSettings();
        public SpriteRenderer spriteRenderer;

        void Awake()
        {
            if (shrapnelSettings.prefab && shrapnelSettings.prefab.scene.rootCount > 0) shrapnelSettings.prefab.SetActive(false);

            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

#if UNITY_EDITOR
        void Reset()
        {
            var prefix1 = "Assets/Playniax/Framework/Ignition/Value Pack/Particle Effects/Prefabs/";
            var prefix2 = "Assets/Playniax/Framework/Pyro/Prototyping/Prefabs/Enemies (Bullets)/";

            outroSettings.effectsSettings = new OutroSettings.EffectsSettings[5];

            outroSettings.effectsSettings[0] = new OutroSettings.EffectsSettings();
            outroSettings.effectsSettings[0].prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefix1 + "Explosion Fire (Red).prefab", typeof(GameObject));
            outroSettings.effectsSettings[0].sizeInPixels = 80;
            outroSettings.effectsSettings[0].orderInLayer = 0;

            outroSettings.effectsSettings[1] = new OutroSettings.EffectsSettings();
            outroSettings.effectsSettings[1].prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefix1 + "Explosion Flash (Red).prefab", typeof(GameObject));
            outroSettings.effectsSettings[1].sizeInPixels = 80;
            outroSettings.effectsSettings[1].orderInLayer = 1;

            outroSettings.effectsSettings[2] = new OutroSettings.EffectsSettings();
            outroSettings.effectsSettings[2].prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefix1 + "Explosion Smoke.prefab", typeof(GameObject));
            outroSettings.effectsSettings[2].sizeInPixels = 80;
            outroSettings.effectsSettings[2].orderInLayer = 2;

            outroSettings.effectsSettings[3] = new OutroSettings.EffectsSettings();
            outroSettings.effectsSettings[3].prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefix1 + "Explosion Trails (Red).prefab", typeof(GameObject));
            outroSettings.effectsSettings[3].sizeInPixels = 80;
            outroSettings.effectsSettings[3].orderInLayer = 3;

            outroSettings.effectsSettings[4] = new OutroSettings.EffectsSettings();
            outroSettings.effectsSettings[4].prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefix1 + "Explosion Blastwave.prefab", typeof(GameObject));
            outroSettings.effectsSettings[4].sizeInPixels = 80;
            outroSettings.effectsSettings[4].orderInLayer = 4;

            shrapnelSettings.prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefix2 + "Enemy Bullet (Red).prefab", typeof(GameObject));
        }
#endif

        void Update()
        {
            if (_state == 0)
            {
                Find("Idle").Play(spriteRenderer, .4f);

                var closest = _GetClosest();

                if (closest && Vector3.Distance(closest.transform.position, transform.position) < armedDistance) _state = 1;
            }
            else if (_state == 1)
            {
                var playing = Find("Armed").PlayOnce(spriteRenderer, .08f);

                if (playing == true)
                {
                    _PlayEffects();
                    _Shrapnel();

                    outroSettings.sound.Play();

                    Destroy(gameObject);
                }
            }
        }

        GameObject _GetClosest()
        {
            var list = PlayerGroup.GetList();

            if (list.Count == 0) return null;
            if (list[0] == null) return null;
            if (list[0].gameObject == null) return null;

            if (list.Count == 1 && list[0]) return list[0].gameObject;

            var closest = list[0].gameObject;

            for (int i = 0; i < list.Count; i++)
            {
                if (closest != list[i] && Vector3.Distance(transform.position, list[i].transform.position) < Vector3.Distance(transform.position, closest.transform.position))
                {
                    closest = list[i].gameObject;
                }
            }

            return closest;
        }

        void _PlayEffects()
        {
            int sortingOrder = 0;

            var renderer = GetComponent<Renderer>();
            if (renderer != null) sortingOrder = renderer.sortingOrder;

            for (int i = 0; i < outroSettings.effectsSettings.Length; i++)
            {
                var prefab = outroSettings.effectsSettings[i].prefab;
                if (prefab == null) continue;

                var instance = Instantiate(prefab, transform.position, Quaternion.identity, transform.parent);
                instance.transform.localScale *= outroSettings.effectsSettings[i].scale;

                var instanceRenderer = instance.GetComponent<Renderer>();
                if (instanceRenderer) instanceRenderer.sortingOrder = sortingOrder + outroSettings.effectsSettings[i].orderInLayer;
            }
        }

        void _Shrapnel()
        {
            var renderer = GetComponent<Renderer>();

            for (int i = 0; i < shrapnelSettings.parts; i++)
            {
                var instance = Instantiate(shrapnelSettings.prefab, transform.position, Quaternion.identity, transform.parent);
                if (instance)
                {
                    instance.SetActive(true);

                    var bullet = instance.GetComponent<BulletBase>();
                    if (bullet == null) bullet = instance.AddComponent<BulletBase>();

                    var instanceRenderer = instance.GetComponent<Renderer>();
                    if (renderer && instanceRenderer) instanceRenderer.sortingOrder = renderer.sortingOrder;

                    instance.transform.localScale *= shrapnelSettings.scale;

                    float angle = i * (360f / shrapnelSettings.parts) * Mathf.Deg2Rad;

                    instance.transform.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);

                    bullet.velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * shrapnelSettings.speed;
                }
            }
        }

        int _state;
    }
}