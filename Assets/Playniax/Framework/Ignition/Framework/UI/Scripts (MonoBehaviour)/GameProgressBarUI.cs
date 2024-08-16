using UnityEngine;

namespace Playniax.Ignition.UI
{
    // Displays game progress based on the player's progress in leveling up.
    public class GameProgressBarUI : MonoBehaviour
    {
        public enum Mode { Horizontal, Vertical };

        [Tooltip("Mode of operation: Horizontal or Vertical.")]
        public Mode mode;

        private Transform _transform;

        void Awake()
        {
            _transform = GetComponent<Transform>();
            if (_transform == null)
            {
                Debug.LogError("GameProgressBarUI: Transform component is missing.");
            }
        }

        public void SetProgress(int current, int max)
        {
            if (_transform == null) return;

            float scale = 1.0f / max * current;

            if (scale < 0) scale = 0;
            if (scale > 1) scale = 1;

            if (mode == Mode.Horizontal)
            {
                _transform.localScale = new Vector3(scale, _transform.localScale.y, _transform.localScale.z);
            }
            else
            {
                _transform.localScale = new Vector3(_transform.localScale.x, scale, _transform.localScale.z);
            }
        }
    }
}