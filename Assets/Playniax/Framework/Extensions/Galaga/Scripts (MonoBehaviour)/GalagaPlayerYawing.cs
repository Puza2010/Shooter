using UnityEngine;

namespace Playniax.Galaga
{
    public class GalagaPlayerYawing : MonoBehaviour
    {
        public float sensitivity = .1f;
        public Sprite[] sprites;
        public bool inverse = true;

        public SpriteRenderer spriteRenderer;

        void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (Time.deltaTime == 0) return;

            var velocity = (transform.position - _previousPosition) / Time.deltaTime;

            velocity.x *= sensitivity;

            if (velocity.x < -1) velocity.x = -1;
            if (velocity.x > 1) velocity.x = 1;

            if (sprites.Length > 0)
            {
                var idle = sprites.Length / 2;

                int frame = idle - (int)(idle * velocity.x * (inverse ? -1 : 1));

                spriteRenderer.sprite = sprites[frame];
            }

            _previousPosition = transform.position;
        }

        Vector3 _previousPosition;
    }
}