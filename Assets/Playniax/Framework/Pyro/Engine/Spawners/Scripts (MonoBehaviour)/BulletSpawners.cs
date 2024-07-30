#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class BulletSpawners : BulletSpawnerBase
    {
#if UNITY_EDITOR

        // Not finished!

        [CustomEditor(typeof(BulletSpawners))]
        public class BulletSpawnersEditor : Editor
        {
            void OnSceneGUI()
            {
                var bulletSpawners = (BulletSpawners)target;

                if (bulletSpawners.drawGizmos && bulletSpawners.spawnPoints.Length > 0 && Selection.activeGameObject == bulletSpawners.gameObject)
                {
                    float size = .05f;
                    Vector3 snap = Vector3.one * 0.5f;

                    for (int i = 0; i < bulletSpawners.spawnPoints.Length; i++)
                    {
                        var fmh_30_129_638443039838825380 = Quaternion.identity; bulletSpawners.spawnPoints[i].position = Handles.FreeMoveHandle(bulletSpawners.spawnPoints[i].position, size, snap, Handles.RectangleHandleCap);

                        var renderer = bulletSpawners.prefab.GetComponent<SpriteRenderer>();
                        var sprite = renderer.sprite;
                        var position = bulletSpawners.spawnPoints[i].position;

                        Handles.Label(position, sprite.texture);
                    }
                }
            }
        }
#endif
        [System.Serializable]
        public class SpawnPoints
        {
            public string name;
            public int group;
            public Vector3 position;
            public Vector3 rotation;
            public float speed = 16;
        }

        [System.Serializable]
        public class CollisionSettings
        {
            public bool useTheseSettings;
            public float structuralIntegrity = 1;
        }

        [System.Serializable]
        public class EffectsSettings
        {
            public GameObject prefab;
            public float scale = 1;
        }

        [System.Serializable]
        public class PowerSettings
        {
            public bool useTheseSettings;
            public float powerRange = 1000;
            public bool visualize;
            public float visualizeScale = .25f;
        }

        [System.Serializable]
        public class TriggerSettings
        {
#if UNITY_EDITOR
            [CustomPropertyDrawer(typeof(TriggerSettings))]
            public class Drawer : PropertyDrawer
            {
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    Rect rect;

                    float y = 0;

                    EditorGUI.BeginProperty(position, label, property);

                    var indent = EditorGUI.indentLevel;

                    rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("mode"));

                    if (property.FindPropertyRelative("mode").enumValueIndex == 1)
                    {
                        EditorGUI.indentLevel += 1;

                        rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(rect, property.FindPropertyRelative("Button1"));
                        rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(rect, property.FindPropertyRelative("Button2"));
                        rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(rect, property.FindPropertyRelative("autofire"));

                        if (property.FindPropertyRelative("autofire").boolValue == true)
                        {
                            EditorGUI.indentLevel = 2;

                            rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                            EditorGUI.PropertyField(rect, property.FindPropertyRelative("rapidFire"));
                        }
                    }

                    EditorGUI.indentLevel = indent;

                    EditorGUI.EndProperty();
                }

                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    float totalLines = 1;

                    if (property.FindPropertyRelative("mode").enumValueIndex == 1)
                    {
                        totalLines += 3;

                        if (property.FindPropertyRelative("autofire").boolValue == true)
                        {
                            totalLines += 1;
                        }
                    }

                    return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * totalLines;
                }
            }
#endif
            public enum Mode { AlwaysFire, ControlledFire, SmartFire };

            public Mode mode;
            public KeyCode Button1 = KeyCode.JoystickButton0;
            public KeyCode Button2 = KeyCode.Space;
            public bool autofire;
            public bool rapidFire;
        }

        public int group;
        public GameObject prefab;
        public Transform parent;
        public int layer;
        public bool inheritOrderInLayer = true;
        public bool friendlyFire = true;
        public float scale = 1;
        public Vector3 rotation;

        public SpawnPoints[] spawnPoints = new SpawnPoints[1];

        [Header("Trigger Settings")]
        public TriggerSettings triggerSettings;
        public AudioProperties audioProperties;
        public CollisionSettings overrideCollisionSettings;
        public PowerSettings powerSettings = new PowerSettings();
        public EffectsSettings effectsSettings = new EffectsSettings();

#if UNITY_EDITOR
        public bool drawGizmos;
#endif
        public override void UpdateSpawner()
        {
            if (prefab == null) return;

            if (triggerSettings.mode == TriggerSettings.Mode.AlwaysFire)
            {
                if (timer.Update()) OnSpawn();
            }
            else if (triggerSettings.mode == TriggerSettings.Mode.SmartFire && BulletSpawnerHelper.count > 0)
            {
                if (timer.Update()) OnSpawn();
            }
            else if (triggerSettings.mode == TriggerSettings.Mode.ControlledFire)
            {
                if (triggerSettings.autofire)
                {
                    if (Input.GetKey(triggerSettings.Button1) || Input.GetKey(triggerSettings.Button2))
                    {
                        if (timer.Update()) OnSpawn();
                    }
                    else if (triggerSettings.rapidFire == true)
                    {
                        timer.timer = 0;
                    }
                }
                else
                {
                    if (Input.GetKeyDown(triggerSettings.Button1) || Input.GetKeyDown(triggerSettings.Button2))
                    {
                        if (timer.Countdown()) OnSpawn();
                    }
                }
            }
        }
        public override void OnInitialize()
        {
            if (prefab && prefab.scene.rootCount > 0) prefab.SetActive(false);
            if (effectsSettings.prefab && effectsSettings.prefab.scene.rootCount > 0) effectsSettings.prefab.SetActive(false);
        }
        public override void OnSpawn()
        {
            for (int i = 0; i < spawnPoints.Length; i++)
                if (spawnPoints[i].group == group) OnSpawn(i);

            audioProperties.Play();
        }

        public void OnSpawn(int i)
        {
            var instance = Instantiate(prefab, transform.position, transform.rotation);

            if (instance)
            {
                if (instance.layer != layer) instance.layer = layer;

                instance.transform.Rotate(rotation);
                instance.transform.Rotate(spawnPoints[i].rotation);
                instance.transform.localScale *= scale;
                instance.transform.Translate(spawnPoints[i].position);

                var bulletBase = instance.GetComponent<BulletBase>();

                if (bulletBase) bulletBase.velocity = instance.transform.rotation * new Vector3(spawnPoints[i].speed, 0, 0);

                if (parent)
                {
                    instance.transform.parent = parent;
                }
                else
                {
                    instance.transform.parent = transform.parent;
                }

                var scoreBase = instance.GetComponent<IScoreBase>();

                PyroHelpers.OverrideStructuralIntegrity(prefab.name, instance, scoreBase);

                if (overrideCollisionSettings.useTheseSettings) _OverrideCollisionSettings(instance);

                if (scoreBase != null)
                {
                    if (friendlyFire) scoreBase.friend = gameObject;

                    if (powerSettings.useTheseSettings)
                    {
                        if (timer.counter > 0)
                        {
                            var m = timer.counter / powerSettings.powerRange;

                            scoreBase.structuralIntegrity *= m + 1;

                            if (powerSettings.visualize) instance.transform.localScale *= m * powerSettings.visualizeScale + 1;
                        }
                    }

                    scoreBase.structuralIntegrity *= BulletSpawnerSettings.GetStructuralIntegrityMultiplier();
                }

                if (inheritOrderInLayer) _SortingOrder(instance);

                instance.SetActive(true);

                if (effectsSettings.prefab != null) _Effects(instance);
            }
        }

        void _Effects(GameObject bullet)
        {
            var effects = Instantiate(effectsSettings.prefab, bullet.transform.position, bullet.transform.rotation, bullet.transform);
            effects.transform.localScale *= effectsSettings.scale;
            effects.SetActive(true);
        }

        void _OverrideCollisionSettings(GameObject instance)
        {
            var scoreBase = instance.GetComponent<IScoreBase>();

            if (scoreBase != null) scoreBase.structuralIntegrity = overrideCollisionSettings.structuralIntegrity;
        }
        void _SortingOrder(GameObject instance)
        {
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();

            if (_spriteRenderer)
            {
                var orderInLayer = _spriteRenderer.sortingOrder;
                _spriteRenderer = instance.GetComponent<SpriteRenderer>();
                if (_spriteRenderer != null) _spriteRenderer.sortingOrder = orderInLayer + 1;
            }
        }

        SpriteRenderer _spriteRenderer;
    }
}