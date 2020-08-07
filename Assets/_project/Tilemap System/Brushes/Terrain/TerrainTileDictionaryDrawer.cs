using FeedTheBaby.Tiles.Terrain;

#if UNITY_EDITOR

using FeedTheBaby.Editor.Dictionary;

namespace FeedTheBaby.Editor.Brushes
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