using UnityEngine;

namespace Playniax.Ignition
{
    // Collection of renderer functions.
    public class RendererHelpers
    {
        // Returns the bounds of an object with multiple renderers.
        public static Bounds GetBounds(GameObject gameObject)
        {
            /*
            var bounds = new Bounds(gameObject.transform.position, Vector3.zero);

            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
            */

            Bounds bounds;

            Renderer renderer = gameObject.GetComponent<Renderer>();

            if (renderer != null)
            {
                bounds = renderer.bounds;
            }
            else
            {
                bounds = new Bounds(Vector3.zero, Vector3.zero);
            }

            if (bounds.extents.x == 0)
            {
                bounds = new Bounds(gameObject.transform.position, Vector3.zero);

                foreach (Transform child in gameObject.transform)
                {
                    renderer = child.GetComponent<Renderer>();

                    if (renderer)
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }
                    else
                    {
                        bounds.Encapsulate(GetBounds(child.gameObject));
                    }
                }
            }

            return bounds;
        }

        // Returns the size of an object with multiple renderers.
        public static Vector2 GetSize(GameObject gameObject)
        {
            return GetBounds(gameObject).size;
        }
        public static void IncreaseOrder(GameObject gameObject, int value, bool includeInactive = false)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(includeInactive);

            for (int i = 0; i < renderers.Length; i++)
                renderers[i].sortingOrder += value;
        }

        // Sets order in layer.
        public static void SetOrder(GameObject gameObject, int orderInLayer, bool includeInactive = false)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>(includeInactive);

            for (int i = 0; i < renderers.Length; i++)
                renderers[i].sortingOrder = orderInLayer + i;
        }
    }
}