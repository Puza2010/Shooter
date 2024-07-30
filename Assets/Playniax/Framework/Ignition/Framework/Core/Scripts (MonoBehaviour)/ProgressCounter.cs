using UnityEngine;

namespace Playniax.Ignition
{
    [AddComponentMenu("Playniax/Ignition/ProgressCounter")]
    public class ProgressCounter : MonoBehaviour
    {
        void Awake()
        {
            SpawnerBase.ProgressCounter.Init();
        }

        void OnEnable()
        {
            if (_progressCounter == null) _progressCounter = GetComponent<SpawnerBase.ProgressCounter>();

            if (_progressCounter == null)
            {
                SpawnerBase.ProgressCounter.resetCounter++;

                SpawnerBase.ProgressCounter.Add(1);
            }
        }

        void OnDisable()
        {
            if (_progressCounter == null)
            {
                SpawnerBase.ProgressCounter.resetCounter--;

                GameData.progress -= 1;
            }
        }

        SpawnerBase.ProgressCounter _progressCounter;
    }
}