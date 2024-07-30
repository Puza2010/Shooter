using System.Linq;
using UnityEngine;

namespace Playniax.Ignition.UI
{
    public class ProgressBarUI : MonoBehaviour
    {
        public enum Mode { Horizontal, Vertical };
        public Mode mode;
        public bool reverse;
        public string progressId = "Player 1";

        public IProgressBarUIHelper GetData()
        {
            if (ObjectHelpers.Safe(_data) == false) _data = _GetData();

            return _data;
        }

        void Awake()
        {
            _Update();
        }

        void Update()
        {
            _Update();
        }

        IProgressBarUIHelper _GetData()
        {
            var helpers = FindObjectsOfType<MonoBehaviour>().OfType<IProgressBarUIHelper>();

            foreach (IProgressBarUIHelper data in helpers)
                if (data != null || data.Equals(null) == false && data.progressBarId == progressId) return data;

            return null;
        }

        void _Update()
        {
            if (_transform == null) _transform = GetComponent<Transform>();
            if (_transform == null) return;

            var data = GetData();
            if (data == null) return;

            var max = data.progressBarScale;
            if (max == 0) return;

            var display = data.progressBarDisplay;

            if (reverse) display = max - display;

            var scale = 1.0f / max * display;

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

        IProgressBarUIHelper _data;
        Transform _transform;
    }
}