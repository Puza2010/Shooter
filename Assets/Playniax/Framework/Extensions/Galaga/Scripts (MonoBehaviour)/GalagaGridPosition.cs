using UnityEditor;
using UnityEngine;

namespace Playniax.Galaga
{
    public class GalagaGridPosition : MonoBehaviour
    {
        public bool occupied;

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (occupied == false) Gizmos.DrawIcon(transform.localPosition, "Assets/Playniax/Galaga Essentials/Gizmos/icon.png", true);
        }
#endif
        void Awake()
        {
            _position = transform.transform.localPosition;
        }

        void Update()
        {
            if (GalagaGrid.instance != null) _Update();
        }

        void _Update()
        {
            transform.localPosition += new Vector3(_position.x * -_direction, _direction, 0) * GalagaGrid.GetDeviationSpeed() * Time.deltaTime;

            if (transform.localPosition.y < _position.y - GalagaGrid.GetDeviationRange())
            {
                _direction = -_direction;
            }
            else if (transform.localPosition.y > _position.y)
            {
                transform.localPosition = _position;

                _direction = -_direction;
            }
        }

        int _direction = -1;
        Vector3 _position;
    }
}