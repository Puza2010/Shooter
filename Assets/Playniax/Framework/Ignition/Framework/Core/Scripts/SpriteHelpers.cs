using UnityEngine;

namespace Playniax.Ignition
{
    // A collection of Sprite functions.
    public class SpriteHelpers
    {
        // Changes the color of all SpriteRenderers attached to a given GameObject and its children.
        public static void SetColor(GameObject gameObject, Color color)
        {
            SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();

            for (int i = 0; i < spriteRenderers.Length; i++)
                spriteRenderers[i].color = color;
        }
    }
}