using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace Playniax.Ignition
{
    public class TilemapBoxCollider2DGenerator : MonoBehaviour
    {
        public Tilemap tilemap;

        void Start()
        {
            _GenerateMergedBoxColliders();
        }

        void _GenerateMergedBoxColliders()
        {
            Vector3 cellSize = Vector3.Scale(tilemap.cellSize, tilemap.transform.lossyScale);
            BoundsInt bounds = tilemap.cellBounds;

            HashSet<Vector2Int> processedTiles = new HashSet<Vector2Int>();

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    if (processedTiles.Contains(new Vector2Int(x, y)))
                        continue;

                    TileBase tile = tilemap.GetTile(pos);
                    if (tile != null)
                    {
                        // Calculate the bounds of the current area
                        Vector3 areaMin = tilemap.CellToWorld(pos);
                        Vector3 areaMax = areaMin + cellSize;

                        // Expand the area horizontally
                        int nextX = x + 1;
                        while (nextX < bounds.xMax && tilemap.GetTile(new Vector3Int(nextX, y, 0)) != null)
                        {
                            processedTiles.Add(new Vector2Int(nextX, y));
                            areaMax.x += cellSize.x;
                            nextX++;
                        }

                        // Expand the area vertically
                        int nextY = y + 1;
                        while (nextY < bounds.yMax)
                        {
                            bool canExpand = true;
                            for (int checkX = x; checkX < nextX; checkX++)
                            {
                                if (tilemap.GetTile(new Vector3Int(checkX, nextY, 0)) == null)
                                {
                                    canExpand = false;
                                    break;
                                }
                            }

                            if (canExpand)
                            {
                                for (int checkX = x; checkX < nextX; checkX++)
                                {
                                    processedTiles.Add(new Vector2Int(checkX, nextY));
                                }
                                areaMax.y += cellSize.y;
                                nextY++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        // Create a collider for the current area
                        _CreateBoxCollider(areaMin, areaMax);
                    }
                }
            }
        }

        void _CreateBoxCollider(Vector2 min, Vector2 max)
        {
            GameObject obj = new GameObject("BoxCollider");
            obj.transform.parent = tilemap.transform;

            BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
            collider.offset = (min + max) / 2f;
            collider.size = new Vector2(Mathf.Abs(max.x - min.x), Mathf.Abs(max.y - min.y));
        }
    }
}