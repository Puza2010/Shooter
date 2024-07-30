using UnityEngine;

namespace Playniax.Ignition.UI
{
    // Displays game progress based on spawner and sequencer state.
    public class GameProgressBarUI : MonoBehaviour
    {
        public enum Mode { Horizontal, Vertical };

        [Tooltip("Mode of operation: Horizontal or Vertical.")]
        public Mode mode;

        [Tooltip("If true, applies the operation only to the selected player.")]
        public bool selectedPlayerOnly = true;

        [Tooltip("If true, reverses the operation.")]
        public bool reverse;

        void Awake()
        {
            _Update();
        }

        void Update()
        {
            _Update();
        }

        void _Update()
        {
            _transform = GetComponent<Transform>();
            if (_transform == null) return;

            int progressScale = GameData.progressScale;
            if (progressScale == 0) return;

            int progress = GameData.progress;

            if (reverse) progress = progressScale - progress;

            float scale = 1.0f / progressScale * progress;

            if (scale < 0) scale = 0;
            if (scale > 1) scale = 1;

            if (mode == Mode.Horizontal)
            {
                _transform.localScale = new Vector3(scale, _transform.localScale.y);
            }
            else
            {
                _transform.localScale = new Vector3(_transform.localScale.x, scale);
            }
        }

        Transform _transform;
    }
}