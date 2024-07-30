using UnityEngine;

namespace Playniax.Pyro
{
    // Scales the sprite to fit screen.
    public class SpriteToScreenSize : MonoBehaviour
    {
        public enum Mode { Fill, Horizontal, Vertical, Disabled };

        // Mode can be Mode.Fill, Mode.Horizontal or Mode.Vertical.
        public Mode mode;

        public static void Fit(Mode mode, Transform transform, SpriteRenderer spriteRenderer)
        {
            var cameraSize = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z - Camera.main.transform.position.z)) * 2;
            var spriteSize = spriteRenderer.sprite.bounds.size;

            if (mode == Mode.Fill)
            {
                if (cameraSize.x / spriteSize.x > cameraSize.y / spriteSize.y)
                {
                    transform.localScale = Vector3.one * cameraSize.x / spriteSize.x;
                }
                else
                {
                    transform.localScale = Vector3.one * cameraSize.y / spriteSize.y;
                }
            }
            else if (mode == Mode.Horizontal)
            {
                transform.localScale = Vector3.one * cameraSize.x / spriteSize.x;
            }
            else if (mode == Mode.Vertical)
            {
                transform.localScale = Vector3.one * cameraSize.y / spriteSize.y;
            }
        }

        void Start()
        {
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null) Fit(mode, transform, _spriteRenderer);
        }

        SpriteRenderer _spriteRenderer;
    }
}