#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class DoDamageAndSelfDestruct : CollisionBase2D
    {
        [System.Serializable]
        // Outro Settings determine what effect to play when an object is destroyed.
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
            public AudioProperties audioSettings;
            public bool enabled = true;
        }

        public int playerIndex = -1;
        public int damage = 100;
        public OutroSettings outroSettings;
        public override void OnCollision(CollisionBase2D collision)
        {
            // Check if collision is with the player
            if (collision.CompareTag("Player"))
            {
                // Do not apply damage or effects to the player
                return;
            }

            // Proceed with normal collision handling
            _UpdateState(collision);

            if (outroSettings.enabled)
            {
                _PlayEffects();
                outroSettings.audioSettings.Play();
            }

            Destroy(gameObject);
        }

#if UNITY_EDITOR
        void Reset()
        {
            var prefix = "Assets/Playniax/Framework/Ignition/Value Pack/Particle Effects/Prefabs/";

            outroSettings.effectsSettings = new OutroSettings.EffectsSettings[5];

            outroSettings.effectsSettings[0] = new OutroSettings.EffectsSettings();
            outroSettings.effectsSettings[0].prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefix + "Explosion Fire (Red).prefab", typeof(GameObject));
            outroSettings.effectsSettings[0].sizeInPixels = 80;
            outroSettings.effectsSettings[0].orderInLayer = 0;

            outroSettings.effectsSettings[1] = new OutroSettings.EffectsSettings();
            outroSettings.effectsSettings[1].prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefix + "Explosion Flash (Red).prefab", typeof(GameObject));
            outroSettings.effectsSettings[1].sizeInPixels = 80;
            outroSettings.effectsSettings[1].orderInLayer = 1;

            outroSettings.effectsSettings[2] = new OutroSettings.EffectsSettings();
            outroSettings.effectsSettings[2].prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefix + "Explosion Smoke.prefab", typeof(GameObject));
            outroSettings.effectsSettings[2].sizeInPixels = 80;
            outroSettings.effectsSettings[2].orderInLayer = 2;

            outroSettings.effectsSettings[3] = new OutroSettings.EffectsSettings();
            outroSettings.effectsSettings[3].prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefix + "Explosion Trails (Red).prefab", typeof(GameObject));
            outroSettings.effectsSettings[3].sizeInPixels = 80;
            outroSettings.effectsSettings[3].orderInLayer = 3;

            outroSettings.effectsSettings[4] = new OutroSettings.EffectsSettings();
            outroSettings.effectsSettings[4].prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefix + "Explosion Blastwave.prefab", typeof(GameObject));
            outroSettings.effectsSettings[4].sizeInPixels = 80;
            outroSettings.effectsSettings[4].orderInLayer = 4;
        }
#endif

        void _PlayEffects()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();

            for (int i = 0; i < outroSettings.effectsSettings.Length; i++)
            {
                var prefab = outroSettings.effectsSettings[i].prefab;
                if (prefab == null) continue;

                var scale = GetScale();

                var instance = Instantiate(prefab, transform.position, Quaternion.identity, transform.parent);
                instance.transform.localScale *= scale;

                var instanceRenderer = instance.GetComponent<Renderer>();
                if (instanceRenderer && spriteRenderer != null) instanceRenderer.sortingOrder = spriteRenderer.sortingOrder + outroSettings.effectsSettings[i].orderInLayer;

                float GetScale()
                {
                    if (spriteRenderer == null) return 1;

                    var scale = outroSettings.effectsSettings[i].scale;

                    var sizeInPixels = outroSettings.effectsSettings[i].sizeInPixels;

                    if (sizeInPixels > 0) scale *= Mathf.Max(spriteRenderer.sprite.rect.size.x, spriteRenderer.sprite.rect.size.y) / sizeInPixels;

                    return scale *= Mathf.Max(transform.localScale.x, transform.localScale.y);
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
    }
}