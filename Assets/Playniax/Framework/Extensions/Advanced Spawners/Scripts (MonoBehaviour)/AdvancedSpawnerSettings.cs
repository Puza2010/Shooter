using UnityEngine;

namespace Playniax.Sequencer
{
    public class AdvancedSpawnerSettings : MonoBehaviour
    {
        static AdvancedSpawnerSettings _instance;
        public static float GetStructuralIntegrityMultiplier()
        {
            if (_instance == null) _instance = FindObjectOfType<AdvancedSpawnerSettings>();

            if (_instance) return _instance.structuralIntegrityMultiplier; else return 1;
        }
        public static float GetStructuralIntegrityMultiplierForChild()
        {
            if (_instance == null) _instance = FindObjectOfType<AdvancedSpawnerSettings>();

            if (_instance) return _instance.structuralIntegrityMultiplierForChild; else return 1;
        }

        public float structuralIntegrityMultiplier = 1;
        public float structuralIntegrityMultiplierForChild = 1;
    }
}
