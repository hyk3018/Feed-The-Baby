using UnityEngine.Tilemaps;

namespace FeedTheBaby.Tiles
{
    public enum LevelObjectType
    {
        BERRY_BUSH,
        TREE,
        APPLE_TREE,
        CAMPFIRE,
    }

    [UnityEngine.CreateAssetMenu(fileName = "Level Object Tile", menuName = "Tiles/Level Object Tile", order = 0)]
    public class LevelObjectTile : Tile
    {
        public LevelObjectType levelObjectType;
    }
}