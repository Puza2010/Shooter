using UnityEngine;
using Playniax.Ignition;
using System.Collections.Generic;

namespace Playniax.Pyro
{
    public class RandomScroller : IgnitionBehaviour
    {
        public class Scroll : MonoBehaviour
        {
            public float spin;
            public Vector3 velocity;
            public Vector3 halfSize;

            void Update()
            {
                transform.position += velocity * Time.deltaTime;

                var min = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, transform.position.z - Camera.main.transform.position.z));
                var max = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, transform.position.z - Camera.main.transform.position.z));

                min.x -= halfSize.x;
                max.x += halfSize.x;

                min.y += halfSize.y;
                max.y -= halfSize.y;

                transform.Rotate(0, 0, spin * Time.deltaTime);

                var position = transform.position;

                if (position.x < min.x || position.x > max.x || position.y < max.y || position.y > min.y)
                {
                    if (name.Contains("(Pooled)"))
                    {
                        gameObject.SetActive(false);
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
        public enum Position { Left, Right, Top, Bottom };
        public enum Mode { Random, Edge1, Edge2, EdgeRandom, EdgeDouble, Fixed };

        public Object[] prefab;
        public float timer;
        public string interval = "3";
        public string velocity = "1";
        public string spin;
        public Color color = Color.white;

        public Position enterance = Position.Right;
        public Mode mode = Mode.Random;
        public float edgeOffset;
        [Range(0, 1)]
        public float edgeVary;
        [Range(0, 1)]
        public float edgeVisibility = 1;
        public float fixedPosition;
        public bool randomRotation;
        public string scale = "1";
        public int orderInLayer;
        public float z;
        public int layers;

        public override void OnInitialize()
        {
            for (int i = 0; i < prefab.Length; i++)
            {
                var gameObject = prefab[i] as GameObject;

                if (gameObject && gameObject.scene.rootCount > 0) gameObject.SetActive(false);
            }
        }

        void Update()
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                var velocity = _RandomFloat(this.velocity);

                timer = _RandomFloat(interval); if (velocity > 0) timer /= velocity;

                if (mode != Mode.EdgeDouble)
                {
                    _SetPosition(_Instantiate(Random.Range(0, prefab.Length)), mode, velocity);
                }
                else
                {
                    _SetPosition(_Instantiate(Random.Range(0, prefab.Length)), Mode.Edge1, velocity);
                    _SetPosition(_Instantiate(Random.Range(0, prefab.Length)), Mode.Edge2, velocity);
                }
            }
        }

        GameObject _GetGameObject(string name)
        {
            name += " (Pooled)";

            for (int i = 0; i < _pool.Count; i++)
            {
                if (_pool[i] && _pool[i].activeInHierarchy == false)
                {
                    var o = _pool[i];
                    o.SetActive(true);
                    o.name = name;
                    o.transform.parent = null;
                    o.transform.position = Vector3.zero;
                    o.transform.rotation = Quaternion.identity;
                    o.transform.localScale = Vector3.one;
                    return o;
                }
            }

            var n = new GameObject();
            n.name = name;
            _pool.Add(n);
            return n;
        }

        GameObject _Instantiate(int index)
        {
            GameObject clone = null;

            if (prefab[index] as Sprite)
            {
                var sprite = prefab[index] as Sprite;

                clone = _GetGameObject(sprite.name);

                var spriteRenderer = clone.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null) spriteRenderer = clone.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprite;
                spriteRenderer.sortingOrder = orderInLayer + _layer;
            }
            else if (prefab[index] as GameObject)
            {
                clone = Instantiate(prefab[index] as GameObject, new Vector3(-100000, -100000, -100000), transform.rotation);

                RendererHelpers.SetOrder(clone, orderInLayer + _layer, true);

                clone.SetActive(true);
            }
            else if (prefab[index] as Texture2D)
            {
                var texture = prefab[index] as Texture2D;

                var sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

                clone = new GameObject(texture.name);

                var spriteRenderer = clone.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprite;
                spriteRenderer.sortingOrder = orderInLayer + _layer;
            }

            _layer += 1;

            if (_layer >= layers) _layer = 0;

            clone.transform.localRotation = transform.localRotation;

            clone.transform.parent = transform;

            if (color != Color.white) SpriteHelpers.SetColor(clone, color);

            return clone;
        }
        float _RandomFloat(string str, float defaultValue = 0)
        {
            if (str.Trim() == "") return defaultValue;
            string[] r = str.Split(',');
            if (r.Length == 1) return float.Parse(str, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
            float min = float.Parse(r[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
            float max = float.Parse(r[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
            return Random.Range(min, max);
        }
        void _SetPosition(GameObject gameObject, Mode mode, float velocity)
        {
            var scroll = gameObject.GetComponent<Scroll>();
            if (scroll == null) scroll = gameObject.AddComponent<Scroll>();

            scroll.spin = _RandomFloat(spin);

            gameObject.transform.localScale = Vector3.one * _RandomFloat(this.scale);

            var size = RendererHelpers.GetBounds(gameObject).size;

            scroll.halfSize = size * .5f;

            var min = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0 - Camera.main.transform.position.z));
            var max = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0 - Camera.main.transform.position.z));

            min.x -= scroll.halfSize.x;
            max.x += scroll.halfSize.x;

            min.y += scroll.halfSize.y;
            max.y -= scroll.halfSize.y;

            var position = gameObject.transform.position;
            var scale = gameObject.transform.localScale;

            if (enterance == Position.Left)
            {
                scroll.velocity = Vector3.right;

                position.x = min.x;

                if (mode == Mode.Random)
                {
                    position.y = Random.Range(min.y, max.y);
                }
                if (mode == Mode.Fixed)
                {
                    position.x = fixedPosition;
                }
                else if (mode == Mode.Edge1 || mode == Mode.Edge2 || mode == Mode.EdgeRandom)
                {
                    size.y *= edgeVisibility;

                    size.y *= Random.Range(1 - edgeVary, 1);

                    position.y = max.y + size.y - edgeOffset;

                    if (mode == Mode.Edge2 || mode == Mode.EdgeRandom && Random.Range(0, 2) == 1)
                    {
                        position.y = -position.y;
                        scale.y = -scale.y;

                        gameObject.transform.eulerAngles = -gameObject.transform.eulerAngles;
                    }
                }
            }
            else if (enterance == Position.Right)
            {
                scroll.velocity = Vector3.left;

                position.x = max.x;

                if (mode == Mode.Random)
                {
                    position.y = Random.Range(min.y, max.y);
                }
                if (mode == Mode.Fixed)
                {
                    position.x = fixedPosition;
                }
                else if (mode == Mode.Edge1 || mode == Mode.Edge2 || mode == Mode.EdgeRandom)
                {
                    size.y *= edgeVisibility;

                    size.y *= Random.Range(1 - edgeVary, 1);

                    position.y = max.y + size.y - edgeOffset;

                    if (mode == Mode.Edge2 || mode == Mode.EdgeRandom && Random.Range(0, 2) == 1)
                    {
                        position.y = -position.y;
                        scale.y = -scale.y;

                        gameObject.transform.eulerAngles = -gameObject.transform.eulerAngles;
                    }
                }
            }
            else if (enterance == Position.Top)
            {
                scroll.velocity = Vector3.down;

                position.y = min.y;

                if (mode == Mode.Random)
                {
                    position.x = Random.Range(min.x, max.x);
                }
                if (mode == Mode.Fixed)
                {
                    position.x = fixedPosition;
                }
                else if (mode == Mode.Edge1 || mode == Mode.Edge2 || mode == Mode.EdgeRandom)
                {
                    size.x *= edgeVisibility;

                    size.x *= Random.Range(1 - edgeVary, 1);

                    position.x = min.x + size.x - edgeOffset;

                    if (mode == Mode.Edge2 || mode == Mode.EdgeRandom && Random.Range(0, 2) == 1)
                    {
                        position.x = -position.x;
                        scale.x = -scale.x;

                        gameObject.transform.eulerAngles = -gameObject.transform.eulerAngles;
                    }
                }
            }
            else if (enterance == Position.Bottom)
            {
                scroll.velocity = Vector3.up;

                position.y = max.y;

                if (mode == Mode.Random)
                {
                    position.x = Random.Range(min.x, max.x);
                }
                if (mode == Mode.Fixed)
                {
                    position.x = fixedPosition;
                }
                else if (mode == Mode.Edge1 || mode == Mode.Edge2 || mode == Mode.EdgeRandom)
                {
                    size.x *= edgeVisibility;

                    size.x *= Random.Range(1 - edgeVary, 1);

                    position.x = min.x + size.x - edgeOffset;

                    if (mode == Mode.Edge2 || mode == Mode.EdgeRandom && Random.Range(0, 2) == 1)
                    {
                        position.x = -position.x;
                        scale.x = -scale.x;

                        gameObject.transform.eulerAngles = -gameObject.transform.eulerAngles;
                    }
                }
            }

            position.z = z;

            gameObject.transform.position = position;
            gameObject.transform.localScale = scale;

            if (randomRotation) gameObject.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 359));

            scroll.velocity *= velocity;
        }

        int _layer;
        List<GameObject> _pool = new List<GameObject>();
    }
}