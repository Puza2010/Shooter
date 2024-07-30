using UnityEngine;
using UnityEngine.EventSystems;

namespace Playniax.Ignition
{
    [AddComponentMenu("Playniax/Ignition/Event Systems Helper")]
    // Creates an EventSystem component if not already present in the scene. Useful for ensuring there's only one EventSystem, avoiding conflicts that may arise from multiple EventSystems in a scene.
    public class EventSystemsHelper : MonoBehaviour
    {
        [System.Serializable]
        public class EventSystemSettings
        {
            public GameObject firstSelected;
        }

        public EventSystemSettings eventSystemSettings = new EventSystemSettings();

        void Awake()
        {
            EventSystem eventSystem = FindAnyObjectByType<EventSystem>();

            if (eventSystem == null)
            {
                eventSystem = gameObject.AddComponent<EventSystem>();
                eventSystem.firstSelectedGameObject = eventSystemSettings.firstSelected;
            }

            StandaloneInputModule standAloneInputModule = FindObjectOfType<StandaloneInputModule>();

            if (standAloneInputModule == null) gameObject.AddComponent<StandaloneInputModule>();
        }
    }
}