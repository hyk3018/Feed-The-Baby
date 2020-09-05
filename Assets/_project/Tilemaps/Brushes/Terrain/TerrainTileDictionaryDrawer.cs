#if UNITY_EDITOR

using FeedTheBaby.Editor.Dictionary;
using FeedTheBaby.Tilemaps.Tiles;
using FeedTheBaby.Tilemaps.Tiles.Terrain;

namespace FeedTheBaby.Tilemaps.Brushes
{
    [UnityEditor.CustomPropertyDrawer(typeof(TerrainTileDictionary))]
    public class TerrainTileDictionaryDrawer : SerializableDictionaryDrawer<TerrainType, TerrainTile>
    {
        protected override SerializableKeyValueTemplate<TerrainType, TerrainTile> GetTemplate()
        {
            return GetGenericTemplate<SerializableTerrainTileTemplate>();
        }

        internal class SerializableTerrainTileTemplate : SerializableKeyValueTemplate<TerrainType, TerrainTile>
        {
        }
    }
}

#endif