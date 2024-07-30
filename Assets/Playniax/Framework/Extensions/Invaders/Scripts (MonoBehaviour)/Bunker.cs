using UnityEngine;
using Playniax.Pyro;

public class Bunker : MonoBehaviour
{
    public int size = 32;
    public bool removeRedundantSprites = true;

    public string group = "Obstacle";
    public string material = "Metal";
    public float structuralIntegrity = 1;
    public Material ghostMaterial;

    public CollisionState.OutroSettings.EffectsSettings[] effectsSettings;

    public SpriteRenderer spriteRenderer;

    void Awake()
    {
        _Fragment();
    }

    void _Fragment()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null) return;

        spriteRenderer.enabled = false;

        int width = spriteRenderer.sprite.texture.width / size;
        int height = spriteRenderer.sprite.texture.height / size;

        float x = -(spriteRenderer.sprite.texture.width - size) / 2 / spriteRenderer.sprite.pixelsPerUnit;
        float y = -(spriteRenderer.sprite.texture.height - size) / 2 / spriteRenderer.sprite.pixelsPerUnit;
        
        for (float w = 0; w < width; w++)
        {
            for (float h = 0; h < height; h++)
            {
                var renderer = new GameObject(w * size + "," + h * size + "," + size + "," + size).AddComponent<SpriteRenderer>();
                renderer.transform.parent = transform;
                renderer.transform.localScale = Vector3.one;
                renderer.transform.localPosition = new Vector3(x + w * size / spriteRenderer.sprite.pixelsPerUnit, y + h * size / spriteRenderer.sprite.pixelsPerUnit);
                renderer.color = spriteRenderer.color;

                renderer.sprite = Sprite.Create(spriteRenderer.sprite.texture, new Rect(w * size, h * size, size, size), new Vector2(0.5f, 0.5f), spriteRenderer.sprite.pixelsPerUnit);

                if (collider)
                {
                    BoxCollider2D boxCollider = renderer.gameObject.AddComponent<BoxCollider2D>();

                    if (collider.Distance(boxCollider).isOverlapped == false)
                    {
                        if (removeRedundantSprites) Destroy(boxCollider.gameObject); else Destroy(boxCollider);
                    }
                    else
                    {
                        CollisionState collision = renderer.gameObject.AddComponent<CollisionState>();

                        collision.enabled = false;

                        collision.group = group;
                        collision.structuralIntegrity = structuralIntegrity;
                        collision.ghostMaterial = ghostMaterial;

                        if (effectsSettings != null) collision.outroSettings.effectsSettings = effectsSettings;

                        collision.enabled = true;
                    }
                }
            }
        }

        Destroy(collider);
    }
}
