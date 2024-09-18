using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        public enum Mode { Horizontal, Vertical };

        public string id = "Player 1";
        public Mode mode;
        public bool selectedPlayerOnly = true;
        public bool reverse;

        private Transform _transform;

        void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        void Update()
        {
            _Update();
        }

        void _Update()
        {
            if (_transform == null) return;

            float structuralIntegrity = 0f;
            float maxStructuralIntegrity = 0f;

            var list = PlayerGroup.GetList();
            if (list == null) return;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] && list[i].id == id && list[i].gameObject)
                {
                    // Uncomment the next line if you want to consider only the selected player
                    // if (selectedPlayerOnly && list[i].gameObject != PlayerGroup.GetSelected()) continue;

                    var collisionState = list[i].GetComponent<CollisionState>();
                    if (collisionState != null)
                    {
                        structuralIntegrity += collisionState.structuralIntegrity;
                        maxStructuralIntegrity += collisionState.maxStructuralIntegrity;
                    }
                }
            }

            if (maxStructuralIntegrity == 0) return;

            if (reverse) structuralIntegrity = maxStructuralIntegrity - structuralIntegrity;

            float scale = structuralIntegrity / maxStructuralIntegrity;

            scale = Mathf.Clamp01(scale);

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
