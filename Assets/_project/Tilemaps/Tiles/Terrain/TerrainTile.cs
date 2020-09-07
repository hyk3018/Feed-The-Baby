using UnityEngine.Tilemaps;

namespace FeedTheBaby.Tilemaps.Tiles.Terrain
{
    public enum TerrainType
    {
        GRASS,
        DIRT,
        SAND,
        SAND_BONE,
        PINE,
        BOULDER
    }

    [UnityEngine.CreateAssetMenu(fileName = "Terrain Tile", menuName = "Tiles/Terrain Tile", order = 0)]
    public class TerrainTile : RandomTile
    {
        public TerrainType terrainType;
    }
}