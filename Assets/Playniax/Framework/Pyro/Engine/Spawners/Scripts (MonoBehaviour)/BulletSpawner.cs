#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class BulletSpawner : BulletSpawnerBase
    {
#if UNITY_EDITOR
        [CanEditMultipleObjects]
        [CustomEditor(typeof(BulletSpawner))]
        public class Inspector : Editor
        {
            SerializedProperty directionSettings;
            SerializedProperty pointInSpaceSettings;
            SerializedProperty powerSettings;
            SerializedProperty randomSettings;
            SerializedProperty targetEnemySettings;
            SerializedProperty targetPlayerSettings;
            SerializedProperty effectsSettings;

            SerializedProperty overrideCollisionSettings;

            SerializedProperty audioProperties;

            SerializedProperty timer;
            void OnEnable()
            {
                directionSettings = serializedObject.FindProperty("directionSettings");
                pointInSpaceSettings = serializedObject.FindProperty("pointInSpaceSettings");
                powerSettings = serializedObject.FindProperty("powerSettings");
                randomSettings = serializedObject.FindProperty("randomSettings");
                targetEnemySettings = serializedObject.FindProperty("targetEnemySettings");
                targetPlayerSettings = serializedObject.FindProperty("targetPlayerSettings");
                effectsSettings = serializedObject.FindProperty("effectsSettings");

                audioProperties = serializedObject.FindProperty("audioProperties");

                overrideCollisionSettings = serializedObject.FindProperty("overrideCollisionSettings");
            }
            public override void OnInspectorGUI()
            {
                EditorGUI.BeginChangeCheck();

                serializedObject.Update();

                timer = serializedObject.FindProperty("timer");

                var myScript = target as BulletSpawner;

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(myScript), typeof(BulletSpawner), false);
                EditorGUI.EndDisabledGroup();

                myScript.bootTime = EditorGUILayout.FloatField("Boot Time", myScript.bootTime);
                myScript.automatically = EditorGUILayout.Toggle("Automatically", myScript.automatically);
                EditorGUILayout.PropertyField(timer, new GUIContent("Timer"));
                myScript.allowResidue = EditorGUILayout.Toggle("Allow Residue", myScript.allowResidue);
                myScript.onlyFireBelowMaxVelocity = EditorGUILayout.Toggle("Only Fire Below Max Velocity", myScript.onlyFireBelowMaxVelocity);
                myScript.maxVelocity = EditorGUILayout.FloatField("Max Velocity", myScript.maxVelocity);
                myScript.id = EditorGUILayout.TextField("Id", myScript.id);
                myScript.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", myScript.prefab, typeof(GameObject), true);
                myScript.parent = (Transform)EditorGUILayout.ObjectField("Parent", myScript.parent, typeof(Transform), true);
                myScript.ignoreParent = EditorGUILayout.Toggle("Ignore Parent", myScript.ignoreParent);
                myScript.layer = EditorGUILayout.IntField("Layer", myScript.layer);
                myScript.inheritOrderInLayer = EditorGUILayout.Toggle("Inherit Order In Layer", myScript.inheritOrderInLayer);
                myScript.position = EditorGUILayout.Vector3Field("Position", myScript.position);
                myScript.scale = EditorGUILayout.FloatField("Scale", myScript.scale);
                myScript.speed = EditorGUILayout.FloatField("Speed", myScript.speed);
                myScript.speedRange = EditorGUILayout.FloatField("Speed Range", myScript.speedRange);

                if (myScript.speed < 1) myScript.speed = 1;
                if (myScript.speedRange < 0) myScript.speedRange = 0;

                myScript.friendlyFire = EditorGUILayout.Toggle("Friendly Fire", myScript.friendlyFire);
                myScript.mode = (Mode)EditorGUILayout.EnumPopup("Mode", myScript.mode);

                if (myScript.mode == Mode.Direction)
                {
                    EditorGUILayout.PropertyField(directionSettings, new GUIContent("Direction Settings"));
                }
                else if (myScript.mode == Mode.PointInSpace)
                {
                    EditorGUILayout.PropertyField(pointInSpaceSettings, new GUIContent("Point In Space Settings"));
                }
                else if (myScript.mode == Mode.Random)
                {
                    EditorGUILayout.PropertyField(randomSettings, new GUIContent("Random Settings"));
                }
                else if (myScript.mode == Mode.TargetEnemy)
                {
                    EditorGUILayout.PropertyField(targetEnemySettings, new GUIContent("Target Enemy Settings"));
                }
                else if (myScript.mode == Mode.TargetPlayer)
                {
                    EditorGUILayout.PropertyField(targetPlayerSettings, new GUIContent("Target Player Settings"));
                }

                EditorGUILayout.PropertyField(overrideCollisionSettings, new GUIContent("Override Collision Settings"));
                EditorGUILayout.PropertyField(powerSettings, new GUIContent("Power Settings"));
                EditorGUILayout.PropertyField(effectsSettings, new GUIContent("Effects Settings"));
                EditorGUILayout.PropertyField(audioProperties, new GUIContent("Audio Properties"));

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(myScript);

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
#endif

        [System.Serializable]
        public class CollisionSettings
        {
            public bool useTheseSettings;
            public float structuralIntegrity = 1;
        }

        [System.Serializable]
        public class DirectionSettings
        {
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
                            EditorGUI.indentLevel = 1;

                            rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                            EditorGUI.PropertyField(rect, property.FindPropertyRelative("Button1"));
                            rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                            EditorGUI.PropertyField(rect, property.FindPropertyRelative("Button2"));
                            rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                            EditorGUI.PropertyField(rect, property.FindPropertyRelative("autofire"));

                            if (property.FindPropertyRelative("autofire").boolValue == true)
                            {
                                EditorGUI.indentLevel += 1;

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

            public Vector3 rotation;
            [Header("Trigger Settings")]
            public TriggerSettings triggerSettings = new TriggerSettings();
            public int intensity = 1;
            public int intensitySpread = 10;
            public bool detectTiles = true;
        }

        [System.Serializable]
        public class EffectsSettings
        {
            public GameObject prefab;
            public float scale = 1;
        }

        [System.Serializable]
        public class PointInSpaceSettings
        {
            public Vector3 position;
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
        public class RandomSettings
        {
            public int intensity = 1;
            public int intensitySpread = 10;
        }

        [System.Serializable]
        public class TargetEnemySettings
        {
            public int index;
            public bool toughestFirst;
            public float targetRange;
            public bool detectTiles = true;
        }

        [System.Serializable]
        public class TargetPlayerSettings
        {
            public float targetRange;
            public bool detectTiles = true;
            public int intensity = 1;
            public int intensitySpread = 10;
        }

        [System.Serializable]
        public class Settings
        {
            public enum Mode { TargetPlayer = 0, Random = 2, Direction = 3 };

            public bool useTheseSettings;
            public GameObject prefab;
            public Vector3 position;
            public Mode mode = Mode.TargetPlayer;
            public float scale = 1;
            public float interval = 2;
            public float intervalRange = 6;
            public float speed = 4;
            public float speedRange;
            public int counter = -1;
            public EffectsSettings effectsSettings = new EffectsSettings();
            public AudioProperties audioProperties;
            public CollisionSettings overrideCollisionSettings;
        }

        public enum Mode { TargetPlayer, TargetEnemy, Random, Direction, PointInSpace };

        public GameObject prefab;
        public Transform parent;
        public bool ignoreParent;
        public int layer;
        public bool friendlyFire = true;
        public bool inheritOrderInLayer = true;
        public Mode mode = Mode.TargetPlayer;
        public Vector3 position;
        public float scale = 1;
        public float speed = 8;
        public float speedRange;
        public RandomSettings randomSettings = new RandomSettings();
        public TargetEnemySettings targetEnemySettings = new TargetEnemySettings();
        public TargetPlayerSettings targetPlayerSettings = new TargetPlayerSettings();
        public DirectionSettings directionSettings = new DirectionSettings();
        public PointInSpaceSettings pointInSpaceSettings = new PointInSpaceSettings();
        public CollisionSettings overrideCollisionSettings = new CollisionSettings();
        public AudioProperties audioProperties =  new AudioProperties ();
        public PowerSettings powerSettings = new PowerSettings();
        public EffectsSettings effectsSettings = new EffectsSettings();

        public void Set(Settings settings)
        {
            prefab = settings.prefab;

            scale = settings.scale;

            timer.timer = Random.Range(settings.interval, settings.interval + settings.intervalRange);

            timer.interval = settings.interval;
            timer.intervalRange = settings.intervalRange;
            timer.counter = settings.counter;
            speed = settings.speed;
            speedRange = settings.speedRange;
            audioProperties = settings.audioProperties;
            overrideCollisionSettings = settings.overrideCollisionSettings;

            friendlyFire = true;
            inheritOrderInLayer = true;

            position = settings.position;
            mode = (Mode)settings.mode;

            effectsSettings = settings.effectsSettings;
        }

        public override void UpdateSpawner()
        {
            if (prefab == null) return;

            if (mode == Mode.Direction)
            {
                if (directionSettings.triggerSettings.mode == DirectionSettings.TriggerSettings.Mode.AlwaysFire)
                {
                    if (timer.Update()) OnSpawn();
                }
                else if (directionSettings.triggerSettings.mode == DirectionSettings.TriggerSettings.Mode.SmartFire && BulletSpawnerHelper.count > 0)
                {
                    if (timer.Update()) OnSpawn();
                }
                else if (directionSettings.triggerSettings.mode == DirectionSettings.TriggerSettings.Mode.ControlledFire)
                {
                    if (directionSettings.triggerSettings.autofire)
                    {
                        if (Input.GetKey(directionSettings.triggerSettings.Button1) || Input.GetKey(directionSettings.triggerSettings.Button2))
                        {
                            if (timer.Update()) OnSpawn();
                        }
                        else if (directionSettings.triggerSettings.rapidFire == true)
                        {
                            timer.timer = 0;
                        }
                    }
                    else
                    {
                        if (Input.GetKeyDown(directionSettings.triggerSettings.Button1) || Input.GetKeyDown(directionSettings.triggerSettings.Button2))
                        {
                            if (timer.Countdown()) OnSpawn();
                        }
                    }
                }
            }
            else
            {
                if (timer.Update()) OnSpawn();
            }
        }

        public override void OnInitialize()
        {
            if (prefab && prefab.scene.rootCount > 0) prefab.SetActive(false);
            if (effectsSettings.prefab && effectsSettings.prefab.scene.rootCount > 0) effectsSettings.prefab.SetActive(false);
        }

        public override void OnSpawn()
        {
            if (mode == Mode.TargetPlayer)
            {
                var target = PlayerGroup.GetRandom();
                if (target)
                {
                    if (targetPlayerSettings.detectTiles && TilemapHelpers.RayIntersectingWithTilemap(transform.position, target.transform.position) == true) return;

                    if (targetPlayerSettings.targetRange > 0 && Vector3.Distance(target.transform.position, transform.position) > targetPlayerSettings.targetRange) return;

                    var instance = Instantiate(prefab, transform.position, transform.rotation);
                    if (instance)
                    {
                        if (instance.layer != layer) instance.layer = layer;

                        instance.transform.localScale *= scale;
                        instance.transform.Translate(position, Space.Self);

                        if (ignoreParent)
                        {
                            if (instance.transform.parent) instance.transform.parent = transform.parent.parent;
                        }
                        else
                        {
                            if (parent)
                            {
                                instance.transform.parent = parent;
                            }
                            else
                            {
                                instance.transform.parent = transform.parent;
                            }
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

                        AddIntensity();

                        Aim(instance.gameObject, 0);

                        if (effectsSettings.prefab != null) _Effects(instance);

                        void AddIntensity()
                        {
                            if (targetPlayerSettings.intensity > 0)
                            {
                                var intensity = targetPlayerSettings.intensity - 1;

                                for (int j = 0; j < intensity; j++)
                                {
                                    var pellet = Instantiate(instance, instance.transform.position, instance.transform.rotation, instance.transform.parent);
                                    Aim(pellet, (j + 1) * targetPlayerSettings.intensitySpread);
                                    pellet = Instantiate(instance, instance.transform.position, instance.transform.rotation, instance.transform.parent);
                                    Aim(pellet, (j + 1) * -targetPlayerSettings.intensitySpread);
                                }
                            }
                        }

                        void Aim(GameObject bullet, float offset)
                        {
                            var bulletBase = bullet.GetComponent<BulletBase>();
                            if (bulletBase)
                            {
                                var angle = Mathf.Atan2(target.transform.position.y - bullet.transform.position.y, target.transform.position.x - bullet.transform.position.x);
                                angle += offset * Mathf.Deg2Rad;
                                bullet.transform.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);
                                bulletBase.velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(speed, speed + speedRange);
                            }
                            else
                            {
                                var rb = instance.GetComponent<Rigidbody2D>();
                                if (rb)
                                {
                                    var angle = Mathf.Atan2(target.transform.position.y - bullet.transform.position.y, target.transform.position.x - bullet.transform.position.x);
                                    angle += offset * Mathf.Deg2Rad;
                                    bullet.transform.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);
                                    rb.velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(speed, speed + speedRange);
                                }
                            }
                        }
                    }
                }
            }
            else if (mode == Mode.TargetEnemy)
            {
                var target = Targetable.GetClosest(targetEnemySettings.index, gameObject, targetEnemySettings.toughestFirst, targetEnemySettings.targetRange);
                if (target != null)
                {
                    if (targetEnemySettings.detectTiles == true && TilemapHelpers.RayIntersectingWithTilemap(gameObject.transform.position, target.transform.position) == true) return;

                    var instance = Instantiate(prefab, transform.position, transform.rotation);
                    if (instance)
                    {
                        if (instance.layer != layer) instance.layer = layer;

                        instance.transform.localScale *= scale;
                        instance.transform.Translate(position, Space.Self);

                        if (ignoreParent)
                        {
                            if (instance.transform.parent) instance.transform.parent = transform.parent.parent;
                        }
                        else
                        {
                            if (parent)
                            {
                                instance.transform.parent = parent;
                            }
                            else
                            {
                                instance.transform.parent = transform.parent;
                            }
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

                        Aim();

                        if (effectsSettings.prefab != null) _Effects(instance);

                        void Aim()
                        {
                            var bulletBase = instance.GetComponent<BulletBase>();
                            if (bulletBase)
                            {
                                var angle = Mathf.Atan2(target.gameObject.transform.position.y - instance.transform.position.y, target.gameObject.transform.position.x - instance.transform.position.x);
                                instance.transform.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);
                                bulletBase.velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(speed, speed + speedRange);
                            }
                            else
                            {
                                var rb = instance.GetComponent<Rigidbody2D>();
                                if (rb)
                                {
                                    var angle = Mathf.Atan2(target.gameObject.transform.position.y - instance.transform.position.y, target.gameObject.transform.position.x - instance.transform.position.x);
                                    instance.transform.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);
                                    rb.velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(speed, speed + speedRange);
                                }
                            }
                        }
                    }
                }
            }
            else if (mode == Mode.Random)
            {
                var instance = Instantiate(prefab, transform.position, transform.rotation);
                if (instance)
                {
                    if (instance.layer != layer) instance.layer = layer;

                    instance.transform.localScale *= scale;
                    instance.transform.Translate(position, Space.Self);

                    if (ignoreParent)
                    {
                        if (instance.transform.parent) instance.transform.parent = transform.parent.parent;
                    }
                    else
                    {
                        if (parent)
                        {
                            instance.transform.parent = parent;
                        }
                        else
                        {
                            instance.transform.parent = transform.parent;
                        }
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

                    var angle = Random.Range(0, 359) * Mathf.Deg2Rad;

                    AddIntensity();

                    Aim(instance.gameObject, angle, 0);

                    if (effectsSettings.prefab != null) _Effects(instance);

                    void AddIntensity()
                    {
                        if (targetPlayerSettings.intensity > 0)
                        {
                            var intensity = randomSettings.intensity - 1;

                            for (int j = 0; j < intensity; j++)
                            {
                                var pellet = Instantiate(instance, instance.transform.position, instance.transform.rotation, instance.transform.parent);
                                Aim(pellet, angle, (j + 1) * randomSettings.intensitySpread);
                                pellet = Instantiate(instance, instance.transform.position, instance.transform.rotation, instance.transform.parent);
                                Aim(pellet, angle, (j + 1) * -randomSettings.intensitySpread);
                            }
                        }
                    }

                    void Aim(GameObject bullet, float angle, float offset)
                    {
                        var bulletBase = bullet.GetComponent<BulletBase>();
                        if (bulletBase)
                        {
                            angle += offset * Mathf.Deg2Rad;
                            bullet.transform.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);
                            bulletBase.velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(speed, speed + speedRange);
                        }
                    }
                }
            }
            else if (mode == Mode.Direction)
            {
                var instance = Instantiate(prefab, transform.position, transform.rotation);
                if (instance)
                {
                    if (instance.layer != layer) instance.layer = layer;

                    instance.transform.localScale *= scale;

                    instance.transform.Translate(position, Space.Self);

                    if (ignoreParent)
                    {
                        if (instance.transform.parent) instance.transform.parent = transform.parent.parent;
                    }
                    else
                    {
                        if (parent)
                        {
                            instance.transform.parent = parent;
                        }
                        else
                        {
                            instance.transform.parent = transform.parent;
                        }
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

                    AddIntensity();

                    Aim(instance, 0);

                    if (effectsSettings.prefab != null) _Effects(instance);

                    void AddIntensity()
                    {
                        if (directionSettings.intensity > 0)
                        {
                            var intensity = directionSettings.intensity - 1;

                            for (int j = 0; j < intensity; j++)
                            {
                                var pellet = Instantiate(instance, instance.transform.position, instance.transform.rotation, instance.transform.parent);
                                Aim(pellet, (j + 1) * directionSettings.intensitySpread);
                                pellet = Instantiate(instance, instance.transform.position, instance.transform.rotation, instance.transform.parent);
                                Aim(pellet, (j + 1) * -directionSettings.intensitySpread);
                            }
                        }
                    }

                    void Aim(GameObject bullet, float offset)
                    {
                        var bulletBase = bullet.GetComponent<BulletBase>();
                        if (bulletBase)
                        {
                            var rotation = bullet.transform.rotation * Quaternion.Euler(directionSettings.rotation);
                            var rotated = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z + offset);
                            bullet.transform.rotation = rotated;
                            bulletBase.velocity = rotated * new Vector3(Random.Range(speed, speed + speedRange), 0, 0);
                        }
                        else
                        {
                            var rb = bullet.GetComponent<Rigidbody2D>();
                            if (rb)
                            {
                                var rotation = bullet.transform.rotation;
                                var rotated = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z + offset);
                                bullet.transform.rotation = rotated;
                                rb.velocity = rotated * new Vector3(Random.Range(speed, speed + speedRange), 0, 0);
                            }
                            
                            //Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), instance.GetComponent<Collider2D>());
                        }
                    }
                }
            }
            else if (mode == Mode.PointInSpace)
            {
                var instance = Instantiate(prefab, transform.position, transform.rotation);
                if (instance)
                {
                    if (instance.layer != layer) instance.layer = layer;

                    instance.transform.localScale *= scale;
                    instance.transform.Translate(position, Space.Self);

                    if (ignoreParent)
                    {
                        if (instance.transform.parent) instance.transform.parent = transform.parent.parent;
                    }
                    else
                    {
                        if (parent)
                        {
                            instance.transform.parent = parent;
                        }
                        else
                        {
                            instance.transform.parent = transform.parent;
                        }
                    }

                    var scoreBase = instance.GetComponent<IScoreBase>();

                    if (overrideCollisionSettings.useTheseSettings) _OverrideCollisionSettings(instance);

                    PyroHelpers.OverrideStructuralIntegrity(prefab.name, instance, scoreBase);

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

                    Aim();

                    if (effectsSettings.prefab != null) _Effects(instance);

                    void Aim()
                    {
                        var bulletBase = instance.GetComponent<BulletBase>();
                        if (bulletBase)
                        {
                            var angle = Mathf.Atan2(pointInSpaceSettings.position.y - instance.transform.position.y, pointInSpaceSettings.position.x - instance.transform.position.x);
                            instance.transform.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);
                            bulletBase.velocity = MathHelpers.GetVelocity(instance.transform.position, pointInSpaceSettings.position) * Random.Range(speed, speed + speedRange);
                        }
                    }
                }
            }

            audioProperties.Play();
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
 