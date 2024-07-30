using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Playniax.Ignition;
using Playniax.Pyro;

namespace Playniax.Randomizers
{
    public class RandomLevelGenerator : EngineBehaviour
    {
        public GameObject[] prefab;
        public int layer;
        public int count = 10;
        public Vector3 safeZone;
        public float safeZoneRadius = 3;
        public int failSafe = 1000;
        public float distance = 1;
        public bool trackProgress;
        public Tilemap tilemap;
        public Transform parent;
        public bool useTilemapBounds;
        public Vector3 bounds = new Vector3(5, 5, 0);

#if UNITY_EDITOR
        [Tooltip("Gizmos color settings.")]
        public Color gizmoColor = new Color(0, 1, 0, 0.5f);
        [Tooltip("Whether to show the gizmos or not.")]
        public bool showGizmos = true;
        void OnDrawGizmos()
        {
            if (showGizmos == false) return;

            Gizmos.color = gizmoColor;

            Gizmos.DrawWireSphere(safeZone, safeZoneRadius);
        }
#endif
        public override void OnInitialize()
        {
            SpawnerBase.ProgressCounter.Init();

            for (int i = 0; i < prefab.Length; i++)
                if (prefab[i] && prefab[i].scene.rootCount > 0) prefab[i].SetActive(false);
        }

        public override void OnStart()
        {
            _Generate();
        }
        void _Generate()
        {
            GameObject clone = null;
            List<Bounds> tileBounds = null;

            if (tilemap) tileBounds = TilemapHelpers.GetBounds(tilemap);

            for (int i = 0; i < count; i++)
            {
                var pick = Random.Range(0, prefab.Length);

                var x = Random.Range(-_GetBounds().x, _GetBounds().x);
                var y = Random.Range(-_GetBounds().y, _GetBounds().y);
                var z = Random.Range(-_GetBounds().z, _GetBounds().z);

                if (tilemap)
                {
                    clone = Instantiate(prefab[pick], tilemap.transform);
                }
                else
                {
                    clone = Instantiate(prefab[pick], transform);
                }

                if (clone.layer != layer) clone.layer = layer;

                clone.gameObject.SetActive(true);

                var helper = clone.GetComponent<BoundsHelper>();

                if (helper == null) helper = clone.AddComponent<BoundsHelper>();

                helper.transform.localPosition = new Vector3(x, y, z);

                while (BoundsHelper.IsFreeSpace(helper, safeZone, safeZoneRadius, distance) == false || (tilemap && FreeTileSpace() == false && failSafe > 0))
                {
                    x = Random.Range(-_GetBounds().x, _GetBounds().x);
                    y = Random.Range(-_GetBounds().y, _GetBounds().y);
                    z = Random.Range(-_GetBounds().z, _GetBounds().z);

                    helper.transform.localPosition = new Vector3(x, y, z);

                    failSafe -= 1;
                }

                if (failSafe <= 0)
                {
                    Destroy(clone);

                    return;
                }

                if (parent) clone.transform.SetParent(parent, true);

                var animator = clone.GetComponent<Animator>();
                if (animator) animator.Play(0, -1, Random.Range(0, 1f));

                if (trackProgress)
                {
                    var progress = clone.GetComponent<ProgressCounter>();
                    if (progress == null) progress = clone.AddComponent<ProgressCounter>();
                }

                bool FreeTileSpace()
                {
                    for (int j = 0; j < tileBounds.Count; j++)
                    {
                        var bounds1 = helper.bounds;
                        var bounds2 = tileBounds[j];

                        //bounds1.size *= 2;
                        //bounds2.size *= 2;

                        if (Vector3.Distance(bounds1.center, bounds2.center) < distance) return false;

                        if (bounds1.Intersects(bounds2)) return false;
                    }

                    return true;
                }
            }
        }

        Vector3 _GetBounds()
        {
            if (tilemap && useTilemapBounds)
            {
                var width = Mathf.Max(tilemap.cellBounds.xMin, tilemap.cellBounds.xMax);
                var height = Mathf.Max(tilemap.cellBounds.yMin, tilemap.cellBounds.yMax);

                return new Vector3(width * tilemap.cellSize.x, height * tilemap.cellSize.y);
            }

            return bounds;
        }
    }
}