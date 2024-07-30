#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using Playniax.Pyro;

namespace Playniax.Sequencer
{

    public class Instantiator : SequenceBase
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(Instantiator))]
        public class Inspector : Editor
        {
            override public void OnInspectorGUI()
            {
                var myScript = target as Instantiator;

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(myScript), typeof(Instantiator), false);
                EditorGUI.EndDisabledGroup();

                myScript.useAlternativeSettings = GUILayout.Toggle(myScript.useAlternativeSettings, "Use Custom Settings");

                EditorGUILayout.Space(8);

                myScript.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", myScript.prefab, typeof(GameObject), true);

                if (myScript.useAlternativeSettings)
                {
                    myScript.position = EditorGUILayout.Vector3Field("Position", myScript.position);
                    myScript.rotation = EditorGUILayout.Vector3Field("Rotation", myScript.rotation);
                    myScript.parent = EditorGUILayout.ObjectField("Transform", myScript.parent, typeof(Transform), true) as Transform;

                }
                else
                {
                    myScript.translate = EditorGUILayout.Vector3Field("Translate", myScript.translate);
                }

                myScript.timer = EditorGUILayout.FloatField("Timer", myScript.timer);
            }
        }
#endif
        public GameObject prefab;
        public Vector3 translate;
        public bool useAlternativeSettings;
        public Vector3 position;
        public Vector3 rotation;
        public Transform parent;
        public float timer;
        public override void OnInitialize()
        {
            if (prefab && prefab.scene.rootCount > 0) prefab.SetActive(false);
        }

        public override void OnSequencerAwake()
        {
            ProgressCounter.Add(1);
        }

        public override void OnSequencerUpdate()
        {
            if (timer > 0)
            {
                timer -= 1 * Time.deltaTime;
            }
            else
            {
                OnSpawn();

                enabled = false;
            }
        }

        public virtual GameObject OnSpawn()
        {
            GameObject instance;

            if (useAlternativeSettings)
            {
                instance = Instantiate(prefab, position, Quaternion.Euler(rotation), parent);
            }
            else
            {
                instance = Instantiate(prefab, position, prefab.transform.rotation, null);

                instance.transform.Translate(translate);
            }

            instance.AddComponent<Register>();
            instance.AddComponent<ProgressCounter>();

            var scoreBase = instance.GetComponent<IScoreBase>();
            if (scoreBase != null)
            {
                PyroHelpers.OverrideStructuralIntegrity(prefab.name, instance, scoreBase);

                scoreBase.structuralIntegrity *= AdvancedSpawnerSettings.GetStructuralIntegrityMultiplier();
            }

            instance.SetActive(true);

            return instance;
        }
    }
}