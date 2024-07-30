using UnityEngine;
using UnityEngine.Tilemaps;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    [DefaultExecutionOrder(1000)]
    public class PyroTileCollider2D : MonoBehaviour
    {
        public bool autopopulateCollisionState = true;
        public bool useTilemapCollider = true;
        public bool fallbackToGenericMode = true;
        public CollisionState collisionState;
        public Vector3 blockSize = new Vector3(1, 1, 0);
        public Vector3 offset;
        public int delay = 1;
        public bool tearMode;
        public float tearPower = .25f;

#if UNITY_EDITOR
        [Tooltip("Gizmos color settings.")]
        public Color gizmoColor = new Color(1, 0, 1, 0.5f);
        [Tooltip("Whether to show the gizmos or not.")]
        public bool showGizmos = true;

        void OnDrawGizmos()
        {
            if (showGizmos == false) return;

            Gizmos.color = gizmoColor;

            Gizmos.DrawWireCube(transform.position + offset, blockSize * .5f);
        }
#endif

        void OnEnable()
        {
            _frameStart = Time.frameCount + delay;

            if (autopopulateCollisionState && collisionState == null) collisionState = GetComponent<CollisionState>();
        }

        void OnCollisionEnter2D()
        {
        }

        void Update()
        {
            _Update();
            //_Test();
        }

        void _Test()
        {
            var pyroTilemapColliders = PyroTilemapCollider2D.Get();

            var tilemap = pyroTilemapColliders[0].GetTilemap();

            ContactPoint2D[] contacts = new ContactPoint2D[10];
            int contactCount = collisionState.colliders[0].GetContacts(contacts);

            for (int i = 0; i < contactCount; i++)
            {
                ContactPoint2D contact = contacts[i];

                Vector3 hitPosition = Vector3.zero;
                hitPosition.x = contact.point.x - 0.01f * contact.normal.x;
                hitPosition.y = contact.point.y - 0.01f * contact.normal.y;

                Vector3Int cellPosition = tilemap.WorldToCell(hitPosition);
                TileBase hitTile = tilemap.GetTile(cellPosition);

                if (hitTile != null)
                {
                    //Debug.Log("Hit tile at position: " + cellPosition + " with tile: " + hitTile.name);

                    tilemap.SetTile(cellPosition, null);
                }
            }
        }

        void _Update()
        {
            if (collisionState == null) return;
            if (Time.frameCount <= _frameStart) return;

            var pyroTilemapColliders = PyroTilemapCollider2D.Get();

            for (int i = 0; i < pyroTilemapColliders.Length; i++)
            {
                if (pyroTilemapColliders[i] == null) continue;
                if (pyroTilemapColliders[i].enabled == false) continue;

                var tilemap = pyroTilemapColliders[i].GetTilemap();

                if (tilemap == null) continue;

                var pyroTilemapCollider = pyroTilemapColliders[i].GetTilemapCollider2D();

                if (useTilemapCollider == true)
                {
                    if (pyroTilemapCollider != null && _CollidersOverlap(pyroTilemapCollider) == false) continue;
                }

                var tilesBlock = TilemapHelpers.GetTiles(tilemap, transform.position + offset, blockSize);
                var positions = TilemapHelpers.GetPositions(tilemap, transform.position + offset, blockSize);

                for (int j = 0; j < tilesBlock.Length; j++)
                {
                    if (tilesBlock[j] != null)
                    {
                        var pyroTiles = tilesBlock[j] as PyroTile;

                        if (pyroTiles != null && pyroTiles.collisionSettings.enabled == true)
                        {
                            pyroTiles = pyroTiles.GetClone(tilemap, tilemap.WorldToCell(positions[j]));

                            if (pyroTiles.collisionSettings.indestructible == true)
                            {
                                collisionState.Kill();
                            }
                            else
                            {
                                Damage();
                            }

                            return;
                        }

                        void Damage()
                        {
                            if (tearMode)
                            {
                                pyroTiles.collisionSettings.structuralIntegrity -= tearPower;
                                collisionState.structuralIntegrity -= tearPower;
                            }
                            else
                            {
                                var d1 = pyroTiles.collisionSettings.structuralIntegrity;
                                var d2 = collisionState.structuralIntegrity;

                                if (d2 > 0) pyroTiles.collisionSettings.structuralIntegrity -= d2;
                                if (d1 > 0) collisionState.structuralIntegrity -= d1;
                            }

                            if (pyroTiles.collisionSettings.structuralIntegrity <= 0)
                            {
                                Tile tile = null;

                                if (pyroTiles.inRuinsSettings.sprite != null)
                                {
                                    tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
                                    tile.sprite = pyroTiles.inRuinsSettings.sprite;
                                    tile.color = pyroTiles.inRuinsSettings.color;
                                    tile.colliderType = Tile.ColliderType.None;
                                }

                                tilemap.SetTile(tilemap.WorldToCell(positions[j]), tile);
                                //tilemap.RefreshTile(tilemap.WorldToCell(positions[j]));

                                pyroTiles.outroSettings.Play(tilemap.GetCellCenterWorld(tilemap.WorldToCell(positions[j])), tilemap.transform, Mathf.Max(tilemap.cellSize.x, tilemap.cellSize.y));

                                pyroTiles.outroSettings.audioSettings.Play();
                            }

                            if (collisionState.isActiveAndEnabled == true)
                            {
                                if (collisionState.structuralIntegrity <= 0)
                                {
                                    collisionState.Kill();
                                }
                                else
                                {
                                    collisionState.Ghost();
                                }
                            }
                        }
                    }
                }

                if (fallbackToGenericMode && collisionState.indestructible == false) collisionState.Kill();
            }
        }

        bool _CollidersOverlap(TilemapCollider2D tilemapCollider)
        {
            for (int i = 0; i < collisionState.colliders.Length; i++)
                if (tilemapCollider.Distance(collisionState.colliders[i]).isOverlapped == true) return true;

            return false;
        }

        int _frameStart;
    }
}