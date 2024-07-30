using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    [AddComponentMenu("Playniax/Pyro/OffCamera")]
    // Destroys sprite off camera or resets it just outside the camera view for looping.
    public class OffCamera : MonoBehaviour
    {
        public enum Mode { Destroy, Loop, None };
        public enum Directions { All, Left, Right, Top, Bottom };
        [Tooltip("Mode can be Mode.Destroy or Mode.Loop.")]
        public Mode mode;
        [Tooltip("Directions can be Directions.All, Directions.Left, Directions.Right, Directions.Up or Directions.Down.")]
        public Directions directions;
        [Tooltip("A sprite is 'cut off' exactly once it's outside the camera view. With margin you can give it extra space.")]
        public float margin;
        void LateUpdate()
        {
            _Update();
        }
        Bounds _GetBounds(GameObject gameObject)
        {
            Bounds bounds = new Bounds(gameObject.transform.position, Vector3.zero);

            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
                bounds.Encapsulate(renderer.bounds);

            return bounds;
        }
        void _Update()
        {
            Camera camera = Camera.main;
            if (camera == null) return;

            Bounds bounds = _GetBounds(gameObject);
            Vector2 extends = bounds.extents;

            // Quick fix!
            // Min and Max does not take camera position into account so this was creating problems with camera shake.
            // Adding camera position (what could be a solution) was creating round problems so look into this later!
            // -----------------------------------------------
            //if (Camera.main.transform.position.x != 0) return;
            //if (Camera.main.transform.position.y != 0) return;
            // -----------------------------------------------

            var min = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, transform.position.z - Camera.main.transform.position.z));
            var max = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, transform.position.z - Camera.main.transform.position.z));

            min.x -= extends.x;
            max.x += extends.x;

            min.y += extends.y;
            max.y -= extends.y;

            min.x -= margin;
            max.x += margin;

            min.y += margin;
            max.y -= margin;

            var position = transform.position;

            if (mode == Mode.Destroy)
            {
                if (((directions == Directions.Left || directions == Directions.All) && position.x < min.x) ||
                    ((directions == Directions.Right || directions == Directions.All) && position.x > max.x) ||
                    ((directions == Directions.Bottom || directions == Directions.All) && position.y < max.y) ||
                    ((directions == Directions.Top || directions == Directions.All) && position.y > min.y))
                {
                    gameObject.SetActive(false);

                    if (name.Contains(ObjectPooler.MARKER) == false) Destroy(gameObject);
                }
            }
            else if (mode == Mode.Loop)
            {
                if ((directions == Directions.Left || directions == Directions.All) && transform.position.x < min.x)
                {
                    position.x = max.x;

                    transform.position = position;
                }
                else if ((directions == Directions.Right || directions == Directions.All) && transform.position.x > max.x)
                {
                    position.x = min.x;

                    transform.position = position;
                }
                else if ((directions == Directions.Bottom || directions == Directions.All) && transform.position.y < max.y)
                {
                    position.y = min.y;

                    transform.position = position;
                }
                else if ((directions == Directions.Top || directions == Directions.All) && transform.position.y > min.y)
                {
                    position.y = max.y;

                    transform.position = position;
                }
            }
        }
    }
}
