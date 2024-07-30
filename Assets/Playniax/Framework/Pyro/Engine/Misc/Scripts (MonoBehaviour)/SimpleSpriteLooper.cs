using UnityEngine;

namespace Playniax.Pyro
{
    public class SimpleSpriteLooper : MonoBehaviour
    {
        public enum Mode { ScrollDown, ScrollUp };

        public Mode mode;

        public float speed = -10;
        void Start()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();

            var size = spriteRenderer.size * .5f;

            var min = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, transform.position.z - Camera.main.transform.position.z));
            var max = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, transform.position.z - Camera.main.transform.position.z));

            min.x -= size.x;
            max.x += size.x;

            min.y += size.y;
            max.y -= size.y;

            var clone = new GameObject(name).AddComponent<SpriteRenderer>();
            clone.sprite = spriteRenderer.sprite;
            clone.color = spriteRenderer.color;
            clone.sortingOrder = spriteRenderer.sortingOrder;
            clone.transform.localScale = transform.localScale;
            clone.transform.parent = transform;

            var position = transform.position;

            if (mode == Mode.ScrollDown)
            {
                position.x = transform.position.x;
                position.y = min.y;
                position.z = transform.position.z;

                transform.position = position;

                _start = transform.position;

                clone.transform.position = _start - new Vector3(0, spriteRenderer.bounds.extents.y * 2, 0);

                _velocity = new Vector3(0, -speed, 0);
            }
            else if (mode == Mode.ScrollUp)
            {
                position.x = transform.position.x;
                position.y = max.y;
                position.z = transform.position.z;

                transform.position = position;

                _start = transform.position;

                clone.transform.position = _start + new Vector3(0, spriteRenderer.bounds.extents.y * 2, 0);

                _velocity = new Vector3(0, speed, 0);
            }
        }

        void Update()
        {
            var size = GetComponent<SpriteRenderer>().size;

            transform.position += _velocity * Time.deltaTime;

            if (transform.position.y < _start.y - size.y) transform.position = _start;
            if (transform.position.y > _start.y + size.y) transform.position = _start;
        }

        Vector3 _start;
        Vector3 _velocity;
    }
}