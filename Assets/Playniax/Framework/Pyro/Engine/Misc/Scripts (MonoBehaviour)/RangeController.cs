using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    [AddComponentMenu("Playniax/Pyro/RangeController")]
    // Destroys sprite outside the range or resets it just outside the range for looping.
    public class RangeController : MonoBehaviour
    {
        public enum Mode { Destroy, Loop };

        [Tooltip("Mode can be Mode.Destroy or Mode.Loop.")]
        public Mode mode;
        [Tooltip("Determines the range.")]
        public Vector2 range = new Vector2(10, 10);
#if UNITY_EDITOR
        public Color gizmoColor = new Color(1, 0, 1, 0.5f);
        public bool showGizmos = true;
#endif

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (showGizmos == false) return;

            Gizmos.color = gizmoColor;

            Gizmos.DrawWireCube(Camera.main.transform.position, new Vector3(range.x * 2, range.y * 2, 1));
        }
#endif
        void LateUpdate()
        {
            _Update();
        }

        void _Update()
        {
            var size = RendererHelpers.GetSize(gameObject) * .5f;

            var min = new Vector3(-range.x, range.y);
            var max = new Vector3(range.x, -range.y);

            min.x -= size.x;
            max.x += size.x;

            min.y += size.y;
            max.y -= size.y;

            var position = transform.position;

            if (mode == Mode.Destroy)
            {
                if (position.x < min.x ||
                    position.x > max.x ||
                    position.y < max.y ||
                    position.y > min.y)
                {
                    gameObject.SetActive(false);

                    if (name.Contains(ObjectPooler.MARKER) == false) Destroy(gameObject);
                }
            }
            else if (mode == Mode.Loop)
            {
                if (transform.position.x < min.x)
                {
                    position.x = max.x;

                    transform.position = position;
                }
                else if (transform.position.x > max.x)
                {
                    position.x = min.x;

                    transform.position = position;
                }
                else if (transform.position.y < max.y)
                {
                    position.y = min.y;

                    transform.position = position;
                }
                else if (transform.position.y > min.y)
                {
                    position.y = max.y;

                    transform.position = position;
                }
            }
        }
    }
}