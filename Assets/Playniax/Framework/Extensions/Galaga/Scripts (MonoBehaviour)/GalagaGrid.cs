using System.Collections.Generic;
using UnityEngine;
using Playniax.Ignition;

namespace Playniax.Galaga
{
    public class GalagaGrid : MonoBehaviour
    {
        public float deviationRange = .25f;
        public float deviationSpeed = .075f;
        public Vector2 viewportMargin = new Vector2(1.5f, 1.5f);
        public FloatRange randomPointDistance = new FloatRange(2.5f, 10f);

#if UNITY_EDITOR
        public bool drawGizmos = true;
        public Color gizmoColor = Color.cyan;
#endif
        public static GalagaGrid instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<GalagaGrid>();

                return _instance;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (drawGizmos == false) return;

            var viewportSize = GetViewportSize(viewportMargin);

            Gizmos.color = gizmoColor;

            Gizmos.DrawWireCube(transform.position, viewportSize * 2);
        }
#endif

        public static float GetDeviationRange()
        {
            if (instance) return instance.deviationRange; else return .25f;
        }

        public static float GetDeviationSpeed()
        {
            if (instance) return instance.deviationSpeed; else return .075f;
        }

        public static FloatRange GetRandomPointsDistance()
        {
            if (instance) return instance.randomPointDistance; else return new FloatRange(2.5f, 10f);
        }

        public static GalagaGridPosition GetFreePosition()
        {
            if (_gridPositions == null) _gridPositions = FindObjectsOfType<GalagaGridPosition>();

            var positions = new List<GalagaGridPosition>();

            for (int i = 0; i < _gridPositions.Length; i++)
            {
                if (_gridPositions[i].occupied == false) positions.Add(_gridPositions[i]);
            }

            if (positions.Count == 0) return null;

            return positions[Random.Range(0, positions.Count)];
        }

        public static Vector3 GetPosition()
        {
            if (instance) return instance.transform.position; else return default;
        }

        public static Vector2 GetViewportMargin()
        {
            if (instance) return instance.viewportMargin; else return default;
        }

        public static Vector3 GetViewportSize(Vector3 margin = default, float z = 0)
        {
            var size = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, z - Camera.main.transform.position.z));

            size -= margin;

            return size;
        }

        static GalagaGridPosition[] _gridPositions;
        static GalagaGrid _instance;
    }
}