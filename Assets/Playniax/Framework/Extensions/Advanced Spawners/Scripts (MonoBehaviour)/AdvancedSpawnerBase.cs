using UnityEngine;
using Playniax.Pyro;

namespace Playniax.Sequencer
{
    public class AdvancedSpawnerBase : SequenceBase
    {

        [System.Serializable]
        public class AISettings : EnemyAI.CruiserSettings
        {
            public bool enabled = true;
        }

        [System.Serializable]
        public class CargoSettings : CollisionState.CargoSettings
        {
            public enum ReleaseMode { Half, All };

            public ReleaseMode releaseMode;
        }

        [System.Serializable]
        public class ChildSettings
        {
            public GameObject prefab;
            public Vector3 position;
            public float scale = 1;
            public bool random;

            public CollisionSettings overrideCollisionSettings = new CollisionSettings();

            public void Create(GameObject parent)
            {
                var child = Instantiate(prefab, parent.transform.position, Quaternion.identity, parent.transform);

                child.transform.localPosition = position;
                child.transform.localScale *= scale;

                var scoreBase = child.GetComponent<IScoreBase>();

                if (scoreBase != null)
                {
                    PyroHelpers.OverrideStructuralIntegrity(prefab.name, child, scoreBase);

                    if (overrideCollisionSettings.useTheseSettings) scoreBase.structuralIntegrity = overrideCollisionSettings.structuralIntegrity;

                    scoreBase.structuralIntegrity *= AdvancedSpawnerSettings.GetStructuralIntegrityMultiplierForChild();
                }
            }
        }

        [System.Serializable]
        public class CollisionSettings
        {
            public bool useTheseSettings;
            public float structuralIntegrity = 1;
        }

        [System.Serializable]
        public class MarkerSettings
        {
            public GameObject prefab;
            public Vector3 position;
            public float scale = 1;
            public bool colorOverride;
            public Color color = Color.white;

            public CollisionSettings overrideCollisionSettings = new CollisionSettings();

            public void Create(GameObject parent)
            {
                var instance = Instantiate(prefab, parent.transform.position, Quaternion.identity, parent.transform);

                instance.SetActive(true);

                instance.transform.localPosition = position;
                instance.transform.localScale *= scale;

                if (overrideCollisionSettings.useTheseSettings)
                {
                    var scoreBase = instance.GetComponent<IScoreBase>();
                    if (scoreBase != null) scoreBase.structuralIntegrity = overrideCollisionSettings.structuralIntegrity;
                }

                if (colorOverride == true)
                {
                    var renderer = instance.GetComponent<SpriteRenderer>();
                    if (renderer != null) renderer.color = color;
                }
            }
        }

        [System.Serializable]
        public class SurpriseSettings
        {
            public GameObject prefab;
            public float scale = 1;

            public CollisionState.CargoSettings.EffectSettings effectSettings = new CollisionState.CargoSettings.EffectSettings();

            public MarkerSettings markerSettings = new MarkerSettings();
        }

        [Tooltip("Prefabs to use.")]
        public GameObject[] prefabs;
    }
}
