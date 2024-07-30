using UnityEngine;

namespace Playniax.Pyro
{
    public class BulletSpawnerSettings : MonoBehaviour
    {
        static BulletSpawnerSettings _instance;

        public static float GetStructuralIntegrityMultiplier()
        {
            if (_instance == null) _instance = FindObjectOfType<BulletSpawnerSettings>();

            if (_instance) return _instance.structuralIntegrityMultiplier; else return 1;
        }

        public float structuralIntegrityMultiplier = 1;
    }
}