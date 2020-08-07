using UnityEngine;
using UnityEngine.Tilemaps;

namespace FeedTheBaby.Tiles.TerrainDecoration
{
    public class TerrainMap : MonoBehaviour
    {
        [SerializeField] Tilemap tilemap = null;

        public Tilemap TileMap()
        {
            return tilemap;
        }
    }
}