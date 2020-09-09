using UnityEngine;

namespace FeedTheBaby.Tilemaps.Tiles.Terrain
{
    public enum TerrainType
    {
        GRASS,
        DIRT,
        SAND,
        SAND_BONE,
        PINE,
        BOULDER,
        WATER
    }

    [CreateAssetMenu(fileName = "Terrain Tile", menuName = "Tiles/Terrain Tile", order = 0)]
    public class TerrainTile : RuleTile
    {
        public TerrainType terrainType;
    }
}