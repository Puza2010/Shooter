#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class BulletSpawners : BulletSpawnerBase
    {
        public int cannonLevel = 0; // Level of the Cannon skill
        public int threeWayShooterLevel = 0; // Level of the 3 Way Shooter skill
        public int wreckingBallLevel = 0; // Level of the Wrecking Ball skill
        private readonly float wreckingBallDistance = 2f;
        private readonly float wreckingBallSpeed = 3f;
        private float baseInterval;
        public float minInterval = 0.01f; // Minimum firing interval

        private GameObject wreckingBallInstance;  // First wrecking ball instance
        private IScoreBase wreckingBallScoreBase;
        private float wreckingBallPosition;
        private bool isDoubleWreckingBall = false;

        void Awake()
        {
            baseInterval = timer.interval;
            ApplyWeaponSpeedMultiplier();
        }

        public void ApplyWeaponSpeedMultiplier()
        {
            timer.interval = baseInterval / weaponSpeedMultiplier;
            if (timer.interval < minInterval)
                timer.interval = minInterval;
        }
        
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
            
            // Check if WreckingBall skill is active and level is greater than 0
            if (wreckingBallLevel > 0)
            {
                timer.counter = -1; // Infinite firing

                // WreckingBall logic is different, we want to update it instead of spawning bullets
                _UpdateWreckingBall(); // Add this custom method to handle the wrecking ball's orbiting movement
            }
            
            if ((id == "Cannon" && cannonLevel > 0) || 
                (id == "3 Way Shooter" && threeWayShooterLevel > 0) ||
                (id == "Quad Cannons" && timer.counter == -1))
            {
                if (triggerSettings.mode == TriggerSettings.Mode.AlwaysFire)
                {
                    if (timer.Update()) OnSpawn();
                }
            }
            else
            {
                timer.counter = 0;
            }

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
            if (id == "Quad Cannons")
            {
                for (int i = 0; i < spawnPoints.Length; i++)
                {
                    if (spawnPoints[i].group == group)
                    {
                        OnSpawn(i);
                    }
                }
            }
            else
            {
                for (int i = 0; i < spawnPoints.Length; i++)
                {
                    if (spawnPoints[i].group == group)
                    {
                        OnSpawn(i);
                    }
                }
            }

            audioProperties.Play();
        }

        public void OnSpawn(int i)
        {
            var instance = Instantiate(prefab, transform.position, transform.rotation);

            if (instance)
            {
                if (instance.layer != layer) instance.layer = layer;
                
                if (id == "Cannon")
                {
                    AdjustCannonBulletProperties(instance);
                }
                else if (id == "3 Way Shooter")
                {
                    AdjustThreeWayShooterBulletProperties(instance);
                }
                else if (id == "Quad Cannons")
                {
                    AdjustQuadCannonsBulletProperties(instance);
                }

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
        
        void AdjustCannonBulletProperties(GameObject bullet)
        {
            float baseSizeMultiplier = 1f; // Base size for level 1
            float sizeIncrement = 0.2f;      // Increase size per level
            float sizeMultiplier = baseSizeMultiplier + sizeIncrement * (cannonLevel - 1);
            sizeMultiplier = Mathf.Max(sizeMultiplier, baseSizeMultiplier);
            bullet.transform.localScale *= sizeMultiplier;

            float baseLifespan = 0.1f;       // Base lifespan for level 1
            float lifespanIncrement = 0.1f;  // Increase lifespan per level
            float lifespan = baseLifespan + lifespanIncrement * (cannonLevel - 1);
            lifespan = Mathf.Max(lifespan, baseLifespan);
            Destroy(bullet, lifespan);

            var scoreBase = bullet.GetComponent<IScoreBase>();
            if (scoreBase != null)
            {
                float baseStructuralIntegrity = Random.Range(4f, 6f); // Half the damage of main gun level 0
                float integrityIncrement = 2f;      // Increase per level
                float newStructuralIntegrity = baseStructuralIntegrity + integrityIncrement * (cannonLevel - 1);
                scoreBase.structuralIntegrity = newStructuralIntegrity;
            }
        }

        void AdjustThreeWayShooterBulletProperties(GameObject bullet)
        {
            float baseSizeMultiplier = 1f; // Base size for level 1
            float sizeIncrement = 0.2f;      // Increase size per level
            float sizeMultiplier = baseSizeMultiplier + sizeIncrement * (threeWayShooterLevel - 1);
            sizeMultiplier = Mathf.Max(sizeMultiplier, baseSizeMultiplier);
            bullet.transform.localScale *= sizeMultiplier;

            float baseLifespan = 0.1f;       // Base lifespan for level 1
            float lifespanIncrement = 0.1f;  // Increase lifespan per level
            float lifespan = baseLifespan + lifespanIncrement * (threeWayShooterLevel - 1);
            lifespan = Mathf.Max(lifespan, baseLifespan);
            Destroy(bullet, lifespan);

            var scoreBase = bullet.GetComponent<IScoreBase>();
            if (scoreBase != null)
            {
                float baseStructuralIntegrity = Random.Range(2f, 5f); // Half the damage of main gun level 0
                float integrityIncrement = 2f;      // Increase per level
                float newStructuralIntegrity = baseStructuralIntegrity + integrityIncrement * (threeWayShooterLevel - 1);
                scoreBase.structuralIntegrity = newStructuralIntegrity;
            }
        }
        
        void _UpdateWreckingBall()
        {
            // First wrecking ball
            if (wreckingBallInstance == null)
            {
                wreckingBallInstance = Instantiate(prefab, transform);
                wreckingBallInstance.SetActive(true);
                wreckingBallScoreBase = wreckingBallInstance.GetComponent<IScoreBase>();
                if (wreckingBallScoreBase == null) return;
            }

            // Second wrecking ball (for super skill)
            if (isDoubleWreckingBall && wreckingBallInstance2 == null)
            {
                wreckingBallInstance2 = Instantiate(prefab, transform);
                wreckingBallInstance2.SetActive(true);
                wreckingBallScoreBase2 = wreckingBallInstance2.GetComponent<IScoreBase>();
                if (wreckingBallScoreBase2 == null) return;
                
                // Start the second ball at the opposite position
                wreckingBallPosition2 = wreckingBallPosition + Mathf.PI;
            }

            // Update first wrecking ball
            wreckingBallScoreBase.friend = gameObject;

            var playerCollider = gameObject.GetComponent<Collider2D>();
            var wreckingBallCollider = wreckingBallInstance.GetComponent<Collider2D>();

            if (playerCollider != null && wreckingBallCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, wreckingBallCollider);
            }

            float baseDamage = Random.Range(4f, 7f);
            float damageIncrement = 5f;
            wreckingBallScoreBase.structuralIntegrity = baseDamage + damageIncrement * (wreckingBallLevel - 1);

            float speedIncrementPerLevel = 2f;
            float currentSpeed = wreckingBallSpeed + (wreckingBallLevel - 1) * speedIncrementPerLevel;

            wreckingBallPosition += currentSpeed * Time.deltaTime;
            var x = Mathf.Cos(wreckingBallPosition) * wreckingBallDistance;
            var y = Mathf.Sin(wreckingBallPosition) * wreckingBallDistance;
            wreckingBallInstance.transform.localPosition = new Vector3(x, y);

            // Update second wrecking ball if super skill is active
            if (isDoubleWreckingBall && wreckingBallInstance2 != null)
            {
                wreckingBallScoreBase2.friend = gameObject;

                var wreckingBallCollider2 = wreckingBallInstance2.GetComponent<Collider2D>();
                if (playerCollider != null && wreckingBallCollider2 != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, wreckingBallCollider2);
                }

                wreckingBallScoreBase2.structuralIntegrity = baseDamage + damageIncrement * (wreckingBallLevel - 1);

                wreckingBallPosition2 += currentSpeed * Time.deltaTime;
                var x2 = Mathf.Cos(wreckingBallPosition2) * wreckingBallDistance;
                var y2 = Mathf.Sin(wreckingBallPosition2) * wreckingBallDistance;
                wreckingBallInstance2.transform.localPosition = new Vector3(x2, y2);
            }
        }

        GameObject wreckingBallInstance2;  // Second wrecking ball instance
        IScoreBase wreckingBallScoreBase2;
        float wreckingBallPosition2;

        SpriteRenderer _spriteRenderer;

        void AdjustQuadCannonsBulletProperties(GameObject bullet)
        {
            float sizeMultiplier = 2.5f;    // Fixed size for super skill
            bullet.transform.localScale *= sizeMultiplier;

            float lifespan = 0.4f;         // Fixed lifespan for super skill
            Destroy(bullet, lifespan);

            var scoreBase = bullet.GetComponent<IScoreBase>();
            if (scoreBase != null)
            {
                float structuralIntegrity = Random.Range(10f, 15f); // Fixed high damage for super skill
                scoreBase.structuralIntegrity = structuralIntegrity;
            }
        }

        // Add this method to enable/disable double wrecking ball mode
        public void SetDoubleWreckingBall(bool enabled)
        {
            isDoubleWreckingBall = enabled;
            
            if (!enabled && wreckingBallInstance2 != null)
            {
                Destroy(wreckingBallInstance2);
                wreckingBallInstance2 = null;
                wreckingBallScoreBase2 = null;
            }
        }
    }
}