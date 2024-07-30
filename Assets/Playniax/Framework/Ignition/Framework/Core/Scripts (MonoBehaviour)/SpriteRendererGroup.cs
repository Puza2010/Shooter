using UnityEngine;

namespace Playniax.Ignition
{
    [AddComponentMenu("Playniax/Ignition/Sprite Renderer Group")]
    // Manipulates the alpha of all SpriteRenderers within the GameObject and its children.
    public class SpriteRendererGroup : MonoBehaviour
    {
        [Tooltip("The alpha value determines the transparency. A value of 1 means fully opaque, while 0 means fully transparent.")]
        public float alpha = 1;

        void Awake()
        {
            _Init();
        }

        void Start()
        {
            _Init();
        }

        void Update()
        {
            _Update();
        }

        void _GetRenderers()
        {
            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (Contains(spriteRenderers[i]) == false)
                {
                    _spriteRenderers = ArrayHelpers.Add(_spriteRenderers, spriteRenderers[i]);
                    _color = ArrayHelpers.Add(_color, spriteRenderers[i].color);
                }
            }

            bool Contains(SpriteRenderer spriteRenderer)
            {
                for (int i = 0; i < _spriteRenderers.Length; i++)
                    if (_spriteRenderers[i] == spriteRenderer) return true;

                return false;
            }
        }

        void _Init()
        {
            _GetRenderers();

            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                if (_spriteRenderers[i] == null) continue;

                _spriteRenderers[i].color = _color[i] * alpha;
            }
        }

        void _Update()
        {
            _GetRenderers();

            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                if (_spriteRenderers[i] == null) continue;

                Color color = _color[i] * alpha;

                if (color.a < 0) color.a = 0;
                if (color.a > 1) color.a = 1;

                _spriteRenderers[i].color = color;
            }
        }

        Color[] _color = new Color[0];
        SpriteRenderer[] _spriteRenderers = new SpriteRenderer[0];
    }
}