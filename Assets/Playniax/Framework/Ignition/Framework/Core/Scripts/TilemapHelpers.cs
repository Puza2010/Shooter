using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Playniax.Ignition
{
    public class TilemapHelpers
    {
#if UNITY_EDITOR
        public static void DrawGizmo(Tilemap tilemap, Vector3 position, Color positionColor, Vector3 offset, Color offsetColor)
        {
            Vector3 size = Vector3.Scale(tilemap.layoutGrid.cellSize, tilemap.transform.lossyScale);
            var cellPosition = tilemap.WorldToCell(position + Vector3.Scale(offset, size));

            if (tilemap.GetTile(cellPosition) != null) Gizmos.color = offsetColor; else Gizmos.color = positionColor;

            Gizmos.DrawWireCube(tilemap.GetCellCenterWorld(cellPosition), size);
        }
#endif
        public static int CountTiles(Tilemap tilemap, string name)
        {
            int count = 0;

            BoundsInt bounds = tilemap.cellBounds;
            TileBase[] tiles = tilemap.GetTilesBlock(bounds);

            for (int i = 0; i < tiles.Length; i++)
                if (tiles[i] && tiles[i].name == name) count += 1;

            return count;
        }

        public static List<Bounds> GetBounds(Tilemap tilemap)
        {
            if (tilemap == null) return null;

            tilemap.CompressBounds();

            BoundsInt bounds = tilemap.cellBounds;

            BoundsInt.PositionEnumerator positions = bounds.allPositionsWithin;

            List<Bounds> list = new List<Bounds>();

            foreach (var position in positions)
            {
                var sprite = tilemap.GetSprite(position);

                if (sprite)
                {
                    Bounds tileBounds = new Bounds(tilemap.CellToWorld(position) + Vector3.Scale(tilemap.cellSize, tilemap.tileAnchor), tilemap.cellSize);

                    list.Add(tileBounds);
                }
            }

            return list;
        }

        // Retrieves a single tile from a Tilemap based on a specified position and offset within a cell.
        public static TileBase GetTile(Tilemap tilemap, Vector3 position, Vector3 offset)
        {
            Vector3 size = Vector3.Scale(tilemap.layoutGrid.cellSize, tilemap.transform.lossyScale);
            Vector3Int cellPosition = tilemap.WorldToCell(position + Vector3.Scale(offset, size));
            return tilemap.GetTile(cellPosition);
        }

        // Retrieves an array of tiles from a Tilemap based on a specified position and size.
        public static TileBase[] GetTiles(Tilemap tilemap, Vector3 position, Vector3 size)
        {
            size = Vector3.Scale(size, tilemap.layoutGrid.cellSize) * .5f;

            Vector3 cellSize = tilemap.layoutGrid.cellSize * .5f;

            TileBase[] tiles = new TileBase[0];

            int i = 0;

            for (float y = -size.y; y <= size.y; y += cellSize.y)
            {
                for (float x = -size.x; x <= size.x; x += cellSize.x)
                {
                    TileBase tile = tilemap.GetTile(tilemap.WorldToCell(position + new Vector3(x, y, 0)));
                    if (tile != null)
                    {
                        Array.Resize(ref tiles, i + 1);

                        tiles[i] = tile;
                    }

                    i += 1;
                }
            }

            return tiles;
        }

        public static Vector3[] GetPositions(Tilemap tilemap, Vector3 position, Vector3 size)
        {
            size = Vector3.Scale(size, tilemap.layoutGrid.cellSize) * .5f;

            Vector3 cellSize = tilemap.layoutGrid.cellSize * .5f;

            Vector3[] positions = new Vector3[0];

            int i = 0;

            for (float y = -size.y; y <= size.y; y += cellSize.y)
            {
                for (float x = -size.x; x <= size.x; x += cellSize.x)
                {
                    Array.Resize(ref positions, i + 1);

                    positions[i] = position + new Vector3(x, y, 0);

                    i += 1;
                }
            }

            return positions;
        }

        // Checks if a ray cast from an origin to a target intersects with any tilemap colliders in the scene.
        public static bool RayIntersectingWithTilemap(Vector3 origin, Vector3 target)
        {
            Vector3 direction = target - origin;
            float distance = Vector3.Distance(target, origin);
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance);

            for (int i = 0; i < hits.Length; i++)
                if (hits[i].collider != null && hits[i].collider.GetComponent<TilemapCollider2D>() != null) return true;

            return false;
        }
    }
}