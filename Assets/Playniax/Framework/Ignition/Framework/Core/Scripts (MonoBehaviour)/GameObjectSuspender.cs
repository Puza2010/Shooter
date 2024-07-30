// Comments by https://chat.openai.com/

using UnityEngine;

namespace Playniax.Ignition
{
    // Suspender class to temporarily suspend the activity of a GameObject
    public class GameObjectSuspender : MonoBehaviour
    {
        // Suspend method to deactivate a GameObject for a specified time
        public static void Suspend(GameObject gameObject, float time)
        {
            // Check if the GameObject or time is invalid, and return if true
            if (gameObject == null || time <= 0) return;

            // Deactivate the GameObject
            gameObject.SetActive(false);

            // Create a new GameObject to handle the suspension and attach the Suspender component
            GameObjectSuspender suspender = new GameObject(gameObject.name + "(Suspended)").AddComponent<GameObjectSuspender>();

            // Set references to the original GameObject and the suspension timer
            suspender._gameObject = gameObject;
            suspender._timer = time;

#if UNITY_EDITOR
            suspender.hideFlags = HideFlags.HideInHierarchy;
#endif
        }

        // LateUpdate is called after all Update functions have been called
        private void LateUpdate()
        {
            // Decrease the timer based on the time that has passed since the last frame
            _timer -= Time.deltaTime;

            // Check if the suspension time has elapsed
            if (_timer < 0)
            {
                // Activate the original GameObject
                _gameObject.SetActive(true);

                // Destroy the GameObject responsible for the suspension
                Destroy(gameObject);
            }
        }

        // Reference to the GameObject being suspended
        private GameObject _gameObject;

        // Timer to control the duration of the suspension
        private float _timer;
    }
}
