using UnityEngine;

namespace Playniax.Pyro
{
    public class RandomSpritePicker : MonoBehaviour
    {
        [Tooltip("Sprites to choose from randomly.")]
        public Sprite[] sprites;

        [Tooltip("Order in layer for the sprite renderer.")]
        public int orderInLayer;

        [Tooltip("Scaling mode for the sprite.")]
        public SpriteToScreenSize.Mode scaleMode = SpriteToScreenSize.Mode.Disabled;

        void Start()
        {
            int index = Random.Range(0, sprites.Length);

            Sprite sprite = sprites[index];

            SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = orderInLayer;

            if (scaleMode != SpriteToScreenSize.Mode.Disabled)
                SpriteToScreenSize.Fit(scaleMode, spriteRenderer.transform, spriteRenderer);
        }
    }
}
