// http://man.hubwiz.com/docset/Unity_3D.docset/Contents/Resources/Documents/docs.unity3d.com/Manual/editor-CustomEditors.html

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    [AddComponentMenu("Playniax/Pyro/EnemyAI")]
    public class EnemyAI : MonoBehaviour
    {
        public static float globalSpeedMultiplier = 1.0f;
        
#if UNITY_EDITOR
        [CanEditMultipleObjects]
        [CustomEditor(typeof(EnemyAI))]
        public class Inspector : Editor
        {
            SerializedProperty cruiserSettings;
            SerializedProperty homingMissileSettings;
            SerializedProperty magnetSettings;
            SerializedProperty tailerSettings;

            void OnEnable()
            {
                cruiserSettings = serializedObject.FindProperty("cruiserSettings");
                homingMissileSettings = serializedObject.FindProperty("homingMissileSettings");
                magnetSettings = serializedObject.FindProperty("magnetSettings");
                tailerSettings = serializedObject.FindProperty("tailerSettings");
            }
            override public void OnInspectorGUI()
            {
                EditorGUI.BeginChangeCheck();

                serializedObject.Update();

                var myScript = target as EnemyAI;

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(myScript), typeof(EnemyAI), false);
                EditorGUI.EndDisabledGroup();

                myScript.mode = (Mode)EditorGUILayout.EnumPopup("Mode", myScript.mode);

                if (myScript.mode == Mode.Cruiser)
                {
                    EditorGUILayout.PropertyField(cruiserSettings, new GUIContent("Cruiser Settings"));
                }
                else if (myScript.mode == Mode.HomingMissile)
                {
                    EditorGUILayout.PropertyField(homingMissileSettings, new GUIContent("Homing Missile Settings"));
                }
                else if (myScript.mode == Mode.Magnet)
                {
                    EditorGUILayout.PropertyField(magnetSettings, new GUIContent("Magnet Settings"));
                }
                else if (myScript.mode == Mode.Tailer)
                {
                    EditorGUILayout.PropertyField(tailerSettings, new GUIContent("Tailer Settings"));
                }

                myScript.target = (GameObject)EditorGUILayout.ObjectField("Target", myScript.target, typeof(GameObject), true);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(myScript);

                    serializedObject.ApplyModifiedProperties();
                }
            }

        }
#endif
        [System.Serializable]
        public class CruiserSettings
        {
            /* Drawer code below is good. Expand to the others first before release!
            #if UNITY_EDITOR
                        [CustomPropertyDrawer(typeof(CruiserSettings))]
                        public class Drawer : PropertyDrawer
                        {
                            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                            {
                                Rect rect;

                                float y = 0;

                                EditorGUI.BeginProperty(position, label, property);

                                var indent = EditorGUI.indentLevel;

                                rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                                EditorGUI.PropertyField(rect, property.FindPropertyRelative("minSpeed"));
                                rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                                EditorGUI.PropertyField(rect, property.FindPropertyRelative("maxSpeed"));
                                rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                                EditorGUI.PropertyField(rect, property.FindPropertyRelative("minReflex"));
                                rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                                EditorGUI.PropertyField(rect, property.FindPropertyRelative("maxReflex"));

                                rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                                EditorGUI.PropertyField(rect, property.FindPropertyRelative("rotateTowards"));

                                if (property.FindPropertyRelative("rotateTowards").boolValue == true)
                                {
                                    EditorGUI.indentLevel += 1;

                                    rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("rotationSpeed"));

                                    EditorGUI.indentLevel = indent;
                                }

                                rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                                EditorGUI.PropertyField(rect, property.FindPropertyRelative("ignoreTarget"));

                                if (property.FindPropertyRelative("ignoreTarget").boolValue == true)
                                {
                                    EditorGUI.indentLevel += 1;

                                    rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("direction"));

                                    rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("directionRange"));

                                    EditorGUI.indentLevel = indent;
                                }

                                //rect = new Rect(position.min.x, position.min.y + y, position.size.x, EditorGUIUtility.singleLineHeight); y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                                //EditorGUI.PropertyField(rect, property.FindPropertyRelative("enabled"));

                                EditorGUI.indentLevel = indent;

                                EditorGUI.EndProperty();
                            }
            #endif
                            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                            {
                                float totalLines = 6;

                                if (property.FindPropertyRelative("rotateTowards").boolValue == true)
                                {
                                    totalLines += 1;
                                }

                                if (property.FindPropertyRelative("ignoreTarget").boolValue == true)
                                {
                                    totalLines += 2;
                                }

                                return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * totalLines;
                            }
                        }
            */
            [Tooltip("Minimum speed of the cruiser.")]
            public float minSpeed = 2;

            [Tooltip("Maximum speed of the cruiser.")]
            public float maxSpeed = 4;

            [Tooltip("Minimum reflex time of the cruiser.")]
            public float minReflex = 1;

            [Tooltip("Maximum reflex time of the cruiser.")]
            public float maxReflex = 3;

            [Tooltip("Determines if the cruiser should rotate towards its movement direction.")]
            public bool rotateTowards;

            [Tooltip("Rotation speed of the cruiser.")]
            public float rotationSpeed = 250;

            [Tooltip("Determines if the cruiser should ignore its target.")]
            public bool ignoreTarget;

            [Tooltip("Direction of the cruiser's movement.")]
            public int direction;

            [Tooltip("Range of direction deviation for the cruiser's movement.")]
            public int directionRange = 360;

            public Vector3 velocity
            {
                get
                {
                    return _speed;
                }
                set
                {
                    _speed = value;
                }
            }
            public void Update(EnemyAI instance)
            {
                if (instance == null) return;
                if (instance.gameObject == null) return;

                if (_timer > 0)
                {
                    _timer -= 1 * Time.deltaTime;
                }
                else
                {
                    _timer = Random.Range(minReflex, maxReflex);

                    float angle;

                    if (ignoreTarget == false && instance.target)
                    {
                        angle = Math2DHelpers.GetAngle(instance.target, instance.gameObject);
                    }
                    else
                    {
                        angle = (direction + Random.Range(-directionRange, directionRange)) * Mathf.Deg2Rad;
                    }

                    _speed += Random.Range(minSpeed, maxSpeed) * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
                }

                instance.transform.position += _speed * EnemyAI.globalSpeedMultiplier * Time.deltaTime;

                if (rotateTowards)
                {
                    var targetRotation = Quaternion.AngleAxis(Mathf.Atan2(_speed.y, _speed.x) * Mathf.Rad2Deg, Vector3.forward);

                    instance.transform.rotation = Quaternion.RotateTowards(instance.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

                _speed *= 1 / (1 + (Time.deltaTime * .99f));
            }

            Vector3 _speed;
            float _timer;
        }

        [System.Serializable]
        public class HomingMissileSettings
        {
            [Tooltip("Time before the missile starts homing in seconds.")]
            public float intro = 0;

            [Tooltip("Speed of the missile during introduction.")]
            public float introSpeed = 8;

            [Tooltip("Speed of the missile.")]
            public float speed = 8;

            [Tooltip("Rotation speed of the missile.")]
            public float rotationSpeed = 250f;

            [Tooltip("Friction applied to the missile's speed.")]
            public float friction;

            public void Update(EnemyAI instance)
            {
                if (instance == null) return;
                if (instance.gameObject == null) return;

                if (intro > 0)
                {
                    speed -= friction * Time.deltaTime;
                    intro -= 1 * Time.deltaTime;
                    if (intro < 0) intro = 0;
                }
                else
                {
                    if (instance.target) _direction = instance.target.transform.position - instance.transform.position;

                    float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

                    var targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

                    instance.transform.rotation = Quaternion.RotateTowards(instance.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

                instance.transform.position += instance.transform.right * speed * EnemyAI.globalSpeedMultiplier * Time.deltaTime;

            }

            Vector3 _direction;
        }

        [System.Serializable]
        public class TailerSettings
        {
            [Tooltip("Speed of the tailer.")]
            public float speed = 1;

            [Tooltip("Enable tailing along the X-axis.")]
            public bool x = true;

            [Tooltip("Enable tailing along the Y-axis.")]
            public bool y = true;

            [Tooltip("Enable tailing along the Z-axis.")]
            public bool z = true;

            [Tooltip("Range of tailing along each axis.")]
            public Vector3 range;
            
            public void Update(EnemyAI instance)
            {
                if (instance == null) return;
                if (instance.gameObject == null) return;
                if (instance.target == null) return;

                //instance.transform.position = Vector3.MoveTowards(instance.transform.position, instance.target.transform.position, Time.deltaTime);

                var target = instance.transform.position;

                if (x) target.x = instance.target.transform.position.x;
                if (y) target.y = instance.target.transform.position.y;
                if (z) target.z = instance.target.transform.position.z;

                if (range.x != 0 && target.x <= instance._startPosition.x - range.x) target.x = instance._startPosition.x - range.x;
                if (range.x != 0 && target.x >= instance._startPosition.x + range.x) target.x = instance._startPosition.x + range.x;

                if (range.y != 0 && target.y <= instance._startPosition.y - range.y) target.y = instance._startPosition.y - range.y;
                if (range.y != 0 && target.y >= instance._startPosition.y + range.y) target.y = instance._startPosition.y + range.y;

                if (range.z != 0 && target.z <= instance._startPosition.z - range.x) target.z = instance._startPosition.z - range.z;
                if (range.z != 0 && target.z >= instance._startPosition.z + range.x) target.z = instance._startPosition.z + range.z;

                instance.transform.position = Vector3.MoveTowards(instance.transform.position, target, speed * EnemyAI.globalSpeedMultiplier * Time.deltaTime);

            }

            //Vector3 _speed;
        }

        [System.Serializable]
        public class MagnetSettings
        {
            // Global polarity affecting all magnets.
            public static int globalPolarity = 1;

            [Tooltip("Range of the magnet's influence.")]
            public float range = 3;

            [Tooltip("Polarity of the magnet.")]
            public int polarity = 1;

            [Tooltip("Base speed of the magnet.")]
            public float speed = 5;

            [Tooltip("Maximum speed of the magnet.")]
            public float maxSpeed = 75;

            [Tooltip("Friction applied to the magnet's speed.")]
            public float friction = 1;

            public void Update(EnemyAI instance)
            {
                if (instance == null) return;
                if (instance.gameObject == null) return;
                if (instance.target == null) return;

                var angle = Math2DHelpers.GetAngle(instance.target, instance.gameObject);
                int distance = (int)(Mathf.Abs(instance.target.transform.position.x - instance.transform.position.x) + Mathf.Abs(instance.target.transform.position.y - instance.transform.position.y));

                if (distance > 0)
                {
                    _speed.x += Mathf.Cos(angle) * (int)(range / distance) * speed * Time.deltaTime;
                    _speed.y += Mathf.Sin(angle) * (int)(range / distance) * speed * Time.deltaTime;
                }

                if (_speed.x > maxSpeed) _speed.x = maxSpeed;
                if (_speed.x < -maxSpeed) _speed.x = -maxSpeed;
                if (_speed.y > maxSpeed) _speed.y = maxSpeed;
                if (_speed.y < -maxSpeed) _speed.y = -maxSpeed;

                instance.transform.position += globalPolarity * polarity * _speed * EnemyAI.globalSpeedMultiplier * Time.deltaTime;


                if (friction != 0) _speed *= 1 / (1 + (Time.deltaTime * friction));
            }

            Vector3 _speed;
        }

        public enum Mode { Cruiser, HomingMissile, Magnet, Tailer };

        [Tooltip("Mode of the behavior.")]
        public Mode mode = Mode.HomingMissile;

        [Tooltip("Settings for the cruiser behavior.")]
        public CruiserSettings cruiserSettings = new CruiserSettings();

        [Tooltip("Settings for the homing missile behavior.")]
        public HomingMissileSettings homingMissileSettings = new HomingMissileSettings();

        [Tooltip("Settings for the magnet behavior.")]
        public MagnetSettings magnetSettings = new MagnetSettings();

        [Tooltip("Settings for the tailer behavior.")]
        public TailerSettings tailerSettings = new TailerSettings();
        
        [Space(8)]
        public GameObject target;

        void Awake()
        {
            _startPosition = transform.position;

            if (mode == Mode.HomingMissile && homingMissileSettings.intro > 0)
            {
                homingMissileSettings.friction = (homingMissileSettings.introSpeed - homingMissileSettings.speed) / homingMissileSettings.intro;
                homingMissileSettings.speed = homingMissileSettings.introSpeed;
            }
        }
        void Update()
        {
            if (target == null || target && target.activeInHierarchy == false) target = PlayerGroup.GetRandom();

            if (mode == Mode.Cruiser)
            {
                cruiserSettings.Update(this);
            }
            else if (mode == Mode.Magnet)
            {
                magnetSettings.Update(this);
            }
            else if (mode == Mode.HomingMissile)
            {
                homingMissileSettings.Update(this);
            }
            else if (mode == Mode.Tailer)
            {
                tailerSettings.Update(this);
            }
        }

        Vector3 _startPosition;

        /*
        void Test1()
        {
            Vector3 direction = target.transform.position - transform.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            transform.position += transform.right * speed * Time.deltaTime;
        }
        */

    }
}