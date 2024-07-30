#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Tilemaps;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    [CreateAssetMenu(fileName = "New Pyro Tile", menuName = "Playniax/Pyro/PyroTile", order = 101)]

    [System.Serializable]
    public class PyroTile : TileBase
    {

#if UNITY_EDITOR
        [CustomEditor(typeof(PyroTile))]
        public class PyroTileEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                var myScript = (PyroTile)target;

                if (myScript.animationSettings.sprites != null)
                {
                    GUILayout.BeginHorizontal();

                    for (int i = 0; i < myScript.animationSettings.sprites.Length; i++)
                    {
                        if (myScript.animationSettings.sprites[i] == null) continue;

                        Texture2D texture = AssetPreview.GetAssetPreview(myScript.animationSettings.sprites[i]);
                        GUILayout.Label("", GUILayout.Height(64), GUILayout.Width(64));
                        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
                    }

                    GUILayout.EndHorizontal();
                }

                DrawDefaultInspector();
            }
        }
#endif

        [System.Serializable]
        public class AnimationSettings
        {
            public Sprite[] sprites = new Sprite[1];
            public float animationSpeed = 10f;
        }

        [System.Serializable]
        public class CollisionSettings
        {
            public Tile.ColliderType colliderType = Tile.ColliderType.Sprite;
            public float structuralIntegrity = 1;
            public bool indestructible;
            public bool enabled = true;
        }

        [System.Serializable]
        public class InRuinsSettings
        {
            public Sprite sprite;
            public Color color = Color.white;
        }

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
            public AudioProperties audioSettings;

            public void Play(Vector3 position, Transform parent, float scale)
            {
                for (int i = 0; i < effectsSettings.Length; i++)
                {
                    var prefab = effectsSettings[i].prefab;
                    if (prefab == null) continue;

                    var instance = Instantiate(prefab, position, Quaternion.identity, parent);
                    instance.transform.localScale *= scale;

                    var instanceRenderer = instance.GetComponent<Renderer>();
                    if (instanceRenderer) instanceRenderer.sortingOrder += effectsSettings[i].orderInLayer;
                }
            }
        }

        public Color color = Color.white;
        public TileFlags flags = TileFlags.None;
        public AnimationSettings animationSettings = new AnimationSettings();
        public CollisionSettings collisionSettings = new CollisionSettings();
        public InRuinsSettings inRuinsSettings = new InRuinsSettings();
        public OutroSettings outroSettings = new OutroSettings();

        public PyroTile GetClone(Tilemap tilemap, Vector3Int position)
        {
            if (_clone == true) return this;

            var tile = Instantiate(this);
            tile._clone = true;
            tilemap.SetTile(position, tile);
            return tile;
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            if (animationSettings.sprites != null && animationSettings.sprites.Length > 0)
            {
                tileData.transform = Matrix4x4.identity;
                tileData.color = color;
                tileData.flags = flags;
                tileData.sprite = animationSettings.sprites[animationSettings.sprites.Length - 1];
                tileData.colliderType = collisionSettings.colliderType;
            }
        }

        public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
        {
            if (animationSettings.sprites != null && animationSettings.sprites.Length > 0)
            {
                tileAnimationData.animatedSprites = animationSettings.sprites;
                tileAnimationData.animationSpeed = animationSettings.animationSpeed;
                tileAnimationData.animationStartTime = Random.Range(0, animationSettings.animationSpeed);
                return true;
            }
            return false;
        }

        /*
        public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject go)
        {
            return true;
        }
        */

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

        bool _clone;
    }
}