using UnityEngine;

namespace Playniax.Ignition
{
    public class ProgressBar : MonoBehaviour
    {
        public enum Mode { Horizontal, Vertical };

        public float max = 100f;
        public float value;
        public Mode mode;

        void Awake()
        {
            _transform = GetComponent<Transform>();

            _Update();
        }

        void Update()
        {
            _Update();
        }

        void _Update()
        {
            if (_transform == null) return;

            float scale = 1f / max * value;

            if (scale < 0) scale = 0;
            if (scale > 1) scale = 1;

            if (mode == Mode.Horizontal)
            {
                _transform.localScale = new Vector2(scale, _transform.localScale.y);
            }
            else
            {
                _transform.localScale = new Vector2(_transform.localScale.x, scale);
            }
        }

        Transform _transform;
    }
}