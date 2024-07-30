using UnityEngine;

namespace Playniax.Galaga
{
    public class GalagaPlayer : MonoBehaviour
    {
        public float speed = 1;
        public float friction = 1f;

        public void Awake()
        {
            _velocity = transform.rotation * Vector3.right * Mathf.Epsilon;
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                var mousePosition = Input.mousePosition;

                if (_previous == Vector3.zero) _previous = mousePosition;

                _velocity += (mousePosition - _previous) * speed * speed * Time.deltaTime;

                _previous = mousePosition;
            }
            else
            {
                _previous = Vector3.zero;
            }

            _velocity.y = 0;

            gameObject.transform.position += _velocity * Time.deltaTime;

            _Bounds();

            if (friction != 0) _velocity *= 1 / (1 + (Time.deltaTime * friction));
        }

        void _Bounds()
        {
            var min = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, transform.position.z - Camera.main.transform.position.z));
            var max = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, transform.position.z - Camera.main.transform.position.z));

            var position = transform.position;

            if (position.x > max.x)
            {
                position.x = max.x;
                _velocity.x = 0;
            }
            else if (position.x < min.x)
            {
                position.x = min.x;
                _velocity.x = 0;
            }

            if (position.y > min.y)
            {
                position.y = min.y;
                _velocity.y = 0;
            }
            else if (position.y < max.y)
            {
                position.y = max.y;
                _velocity.y = 0;
            }

            transform.position = position;
        }

        Vector3 _previous;
        Vector3 _velocity;

    }
}
