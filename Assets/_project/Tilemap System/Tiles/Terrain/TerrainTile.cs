using UnityEngine.Tilemaps;

namespace FeedTheBaby.Tiles.Terrain
{
    public enum TerrainType
    {
        Grassland,
        Desert
    }

    [UnityEngine.CreateAssetMenu(fileName = "Terrain Tile", menuName = "Tiles/Terrain Tile", order = 0)]
    public class TerrainTile : Tile
    {
        public TerrainType terrainType;
    }
}