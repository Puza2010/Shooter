using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        public enum Mode { Horizontal, Vertical };

        public string id = "Player 1";
        public Mode mode;
        public int maximum;
        public bool selectedPlayerOnly = true;
        public bool reverse;
        void Awake()
        {
            _Update();
        }
        void Update()
        {
            _Update();
        }
        int _GetStructuralIntegrity(string id, bool selectedPlayerOnly)
        {
            float count = 0;

            var list = PlayerGroup.GetList();
            if (list == null) return 0;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] && list[i].id == id && list[i].gameObject)
                {
                    //if (selectedPlayerOnly && list[i] && list[i].gameObject != PlayerGroup.GetSelected()) continue;

                    var scoreBase = list[i].GetComponent<IScoreBase>();
                    if (scoreBase != null) count += scoreBase.structuralIntegrity;
                }
            }

            return (int)count;
        }
        void _Update()
        {
            _transform = GetComponent<Transform>();
            if (_transform == null) return;

            var structuralIntegrity = _GetStructuralIntegrity(id, selectedPlayerOnly);

            if (maximum == 0) maximum += structuralIntegrity;
            if (maximum == 0) return;

            if (reverse) structuralIntegrity = maximum - structuralIntegrity;

            var scale = 1.0f / maximum * structuralIntegrity;

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