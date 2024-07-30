using UnityEngine;

namespace Playniax.Ignition
{
    [AddComponentMenu("Playniax/Ignition/DontDestroyOnLoad")]
    // Prevents the GameObject from being destroyed when a new scene is loaded.
    public class DontDestroyOnLoad : MonoBehaviour
    {
        [Tooltip("Determines whether the GameObject should be destroyed on scene load or not")]
        public bool dontDestroyOnLoad = true;

        void Awake()
        {
            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }
    }
}
