using System.Linq;
using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class BoundsHelper : MonoBehaviour, IBoundsHelpers
    {
#if UNITY_EDITOR
        [Tooltip("Gizmos color settings.")]
        public Color gizmoColor = new Color(1, 0, 0, 0.5f);
        [Tooltip("Whether to show the gizmos.")]
        public bool showGizmos = true;
        void OnDrawGizmos()
        {
            if (showGizmos == false) return;

            Gizmos.color = gizmoColor;

            //Gizmos.DrawWireCube(transform.position, bounds.size);
            Gizmos.DrawWireCube(transform.position, new Vector3(bounds.size.x, bounds.size.y, 1));
        }
#endif
        public static bool IsFreeSpace(IBoundsHelpers helper, Vector3 safeZone, float safeZoneRadius, float distance = 1)
        {
            var helpers = FindObjectsOfType<MonoBehaviour>().OfType<IBoundsHelpers>();

            foreach (IBoundsHelpers helper2 in helpers)
            {
                if (helper2.gameObject == helper.gameObject) continue;
                if (Vector3.Distance(safeZone, helper.gameObject.transform.position) < safeZoneRadius) return false;
                if (Vector3.Distance(helper.gameObject.transform.position, helper2.gameObject.transform.position) < distance) return false;

                var bounds1 = helper.bounds;
                var bounds2 = helper2.bounds;

                if (bounds1.Intersects(bounds2)) return false;
            }

            return true;
        }
        public Bounds bounds
        {
            get
            {
                var bounds = RendererHelpers.GetBounds(gameObject);

                var size = bounds.size;

                if (this.size.x != 0) size.x = this.size.x;
                if (this.size.y != 0) size.y = this.size.y;
                if (this.size.z != 0) size.z = this.size.z;

                bounds.size = size;

                return bounds;
            }
        }

        public Vector3 size
        {
            get { return _size; }
            set { _size = value; }
        }

        [SerializeField] Vector3 _size;
    }
}