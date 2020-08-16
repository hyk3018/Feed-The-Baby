using UnityEngine;
using UnityEngine.Tilemaps;

namespace FeedTheBaby.Tilemaps.Brushes
{
    public class TerrainDecorationMap : MonoBehaviour
    {
        [SerializeField] Tilemap tilemap = null;

        public Tilemap TileMap()
        {
            return tilemap;
        }
    }
}