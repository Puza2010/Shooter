using UnityEngine;

namespace Playniax.Ignition
{
    // A collection of camera functions.
    public static class CameraHelpers
    {
        // Returns mouse position.
        public static Vector3 GetMousePosition(Camera camera = null)
        {
            if (camera == null) camera = Camera.main;

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            return ray.GetPoint(-camera.transform.position.z);
        }

        // Returns whether the renderer is visible or not.
        public static bool IsVisible(Renderer renderer, Camera camera = null)
        {
            if (camera == null) camera = Camera.main;

            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }

        // Returns bounds.
        public static Bounds OrthographicBounds(Camera camera = null)
        {
            if (camera == null) camera = Camera.main;

            float aspect = (float)Screen.width / (float)Screen.height;
            float height = camera.orthographicSize * 2;
            return new Bounds(camera.transform.position, new Vector3(height * aspect, height, 0));
        }

        // Returns orthographics width and height.
        public static Vector2 OrthographicSize(Camera camera)
        {
            Vector2 size;

            size.x = camera.orthographicSize * Screen.width / Screen.height;
            size.y = camera.orthographicSize;

            return size;
        }

        // Returns persepctive width and height.
        public static Vector2 PerspectiveSize(Camera camera)
        {
            Vector2 size;

            float halfFOV = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;
            size.y = -camera.transform.position.z * Mathf.Tan(halfFOV);
            size.x = camera.aspect * size.y;

            return size;
        }

        // Returns width and height.
        public static Vector2 Size(Camera camera)
        {
            if (camera.orthographic)
            {
                return OrthographicSize(camera);
            }
            else
            {
                return PerspectiveSize(camera);
            }
        }

        // Sets virtual width.
        public static void SetOrthographicWidth(int virtualWidth, Camera camera = null)
        {
            if (camera == null) camera = Camera.main;

            float width = Screen.width;
            float height = Screen.height;

            float aspect = width / height;

            camera.orthographicSize = (float)virtualWidth / aspect / 200;
        }

        // Sets virtual width and height.
        public static void SetOrthographic(int virtualWidth, int virtualHeight, Camera camera = null)
        {
            if (camera == null) camera = Camera.main;

            float width = Screen.width;
            float height = Screen.height;

            float aspect = width / height;

            float x = (float)virtualWidth / aspect / 200;
            float y = camera.orthographicSize = (float)virtualHeight / 200;

            camera.orthographicSize = Mathf.Max(x, y);
        }

        // Sets virtual height.
        public static void SetOrthographicHeight(int virtualHeight, Camera camera = null)
        {
            if (camera == null) camera = Camera.main;

            camera.orthographicSize = (float)virtualHeight / 200;
        }
    }
}