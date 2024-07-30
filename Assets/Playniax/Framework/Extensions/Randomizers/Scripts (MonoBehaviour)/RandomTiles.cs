using UnityEngine;
using UnityEngine.Tilemaps;
using Playniax.Ignition;

namespace Playniax.Pyro
{
    public class RandomTiles : EngineBehaviour
    {
        public int count = 3;
        public Tilemap[] prefabs;
        public Tilemap tilemap;
        public int failSafe = 100;
        public TileBase voidTile;
        public BoundsInt freeArea;
        public override void OnStart()
        {
            _CompressBounds();

            _DrawForbidden();

            _Fill();

            postInit += _EraseForbidden;
        }

        void _CompressBounds()
        {
            if (tilemap != null) tilemap.CompressBounds();

            for (int i = 0; i < prefabs.Length; i++)
                if (prefabs[i] != null) prefabs[i].CompressBounds();
        }

        void _Copy(Tilemap source, Tilemap target, Vector3Int position)
        {
            var transformMatrix = _GetTransformMatrix(source);

            var bounds = source.cellBounds;
            bounds.position = position;

            var tiles = source.GetTilesBlock(source.cellBounds);
            target.SetTilesBlock(bounds, tiles);

            _SetTransformMatrix(target, bounds, transformMatrix);

            var children = source.GetComponentsInChildren<Transform>();

            for (int i = 0; i < children.Length; i++)
            {
                if (children[i] != source.transform && children[i].parent == source.transform)
                {
                    var localPosition = target.CellToLocal(bounds.position);
                    localPosition += children[i].gameObject.transform.localPosition;
                    localPosition -= target.CellToLocal(source.cellBounds.min);
                    localPosition = Vector3.Scale(localPosition, target.transform.lossyScale);

                    Instantiate(children[i].gameObject, localPosition, children[i].rotation, target.transform);
                }
            }
        }

        void _DrawForbidden()
        {
            _tilesBlock = tilemap.GetTilesBlock(freeArea);

            var tileArray = new TileBase[Mathf.Abs(freeArea.size.x) * Mathf.Abs(freeArea.size.y)];

            for (int i = 0; i < tileArray.Length; i++)
                tileArray[i] = voidTile;

            tilemap.SetTilesBlock(freeArea, tileArray);
        }

        void _EraseForbidden()
        {
            tilemap.SetTilesBlock(freeArea, _tilesBlock);

            foreach (var tilePosition in tilemap.cellBounds.allPositionsWithin)
            {
                var tile = tilemap.GetTile(tilePosition);
                if (tile && tile == voidTile) tilemap.SetTile(tilePosition, null);
            }
        }

        void _Fill()
        {
            var range = tilemap.cellBounds.size / 2;

            for (int i = 0; i < count; i++)
            {
                var prefab = prefabs[Random.Range(0, prefabs.Length)];

                var cellBounds = prefab.cellBounds;

                if (FreeTileSpace()) _Copy(prefab, tilemap, cellBounds.position);

                bool FreeTileSpace()
                {
                    for (int j = 0; j < failSafe; j++)
                    {
                        var x = Random.Range(-range.x, range.x);
                        var y = Random.Range(-range.y, range.y);
                        var z = 0;

                        cellBounds.position = new Vector3Int(x, y, z);

                        if (_IsOccupied() == false) return true;

                        bool _IsOccupied()
                        {
                            var tiles = tilemap.GetTilesBlock(cellBounds);

                            for (int k = 0; k < tiles.Length; k++)
                                if (tiles[k]) return true;

                            return false;
                        }
                    }

                    return false;
                }
            }
        }

        Matrix4x4[] _GetTransformMatrix(Tilemap source)
        {
            var i = 0;

            var transformMatrix = new Matrix4x4[source.cellBounds.size.x * source.cellBounds.size.y * source.cellBounds.size.z];

            for (int x = source.cellBounds.min.x; x < source.cellBounds.max.x; x++)
            {
                for (int y = source.cellBounds.min.y; y < source.cellBounds.max.y; y++)
                    transformMatrix[i++] = source.GetTransformMatrix(new Vector3Int(x, y, 0));
            }

            return transformMatrix;
        }

        void _SetTransformMatrix(Tilemap target, BoundsInt bounds, Matrix4x4[] transformMatrix)
        {
            var i = 0;

            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    target.SetTransformMatrix(new Vector3Int(x, y, 0), transformMatrix[i++]);
                }
            }
        }

        TileBase[] _tilesBlock;
    }
}
