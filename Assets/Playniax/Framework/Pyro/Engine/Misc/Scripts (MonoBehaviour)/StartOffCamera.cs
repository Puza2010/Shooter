using UnityEngine;

namespace Playniax.Pyro
{
    [AddComponentMenu("Playniax/Pyro/StartOffCamera")]
    // Places a sprite just outside the camera view.
    public class StartOffCamera : MonoBehaviour
    {
        public enum StartPosition { Left, Right, Top, Bottom };

        // Can be Left, Right, Top or Bottom.
        public StartPosition startPosition = StartPosition.Left;

        void OnEnable()
        {
            _Init();
        }

        Bounds _GetBounds(GameObject gameObject)
        {
            Bounds bounds = new Bounds(gameObject.transform.position, Vector3.zero);

            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
                bounds.Encapsulate(renderer.bounds);

            return bounds;
        }

        void _Init()
        {
            Camera camera = Camera.main;
            if (camera == null) return;

            Bounds bounds = _GetBounds(gameObject);
            Vector2 extends = bounds.extents;

            Vector3 minViewport = new Vector3(0, 1, transform.position.z - camera.transform.position.z);
            Vector3 maxViewport = new Vector3(1, 0, transform.position.z - camera.transform.position.z);

            Vector3 min = camera.ViewportToWorldPoint(minViewport);
            Vector3 max = camera.ViewportToWorldPoint(maxViewport);

            min.x -= extends.x;
            max.x += extends.x;

            min.y += extends.y;
            max.y -= extends.y;

            Vector3 position = transform.position;

            switch (startPosition)
            {
                case StartPosition.Left:
                    position.x = min.x;
                    transform.position = position;
                    break;
                case StartPosition.Right:
                    position.x = max.x;
                    transform.position = position;
                    break;
                case StartPosition.Top:
                    position.y = min.y;
                    transform.position = position;
                    break;
                case StartPosition.Bottom:
                    position.y = max.y;
                    transform.position = position;
                    break;
            }
        }
    }
}
