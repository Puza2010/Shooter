using UnityEngine;
using UnityEngine.Tilemaps;

namespace Playniax.Pyro
{
    public class PyroTilemapCollider2D : MonoBehaviour
    {
        public static PyroTilemapCollider2D[] Get()
        {
            if (_list == null) _list = FindObjectsOfType<PyroTilemapCollider2D>();

            return _list;
        }

        void OnDisable()
        {
            _list = null;
        }
        public Tilemap GetTilemap()
        {
            if (_tilemap == null) _tilemap = GetComponent<Tilemap>();

            return _tilemap;
        }
        public TilemapCollider2D GetTilemapCollider2D()
        {
            if (_tilemapCollider2D == null) _tilemapCollider2D = GetComponent<TilemapCollider2D>();

            return _tilemapCollider2D;
        }

        static PyroTilemapCollider2D[] _list;

        Tilemap _tilemap;
        TilemapCollider2D _tilemapCollider2D;
    }
}