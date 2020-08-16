using FeedTheBaby.Editor.Dictionary;
using FeedTheBaby.Tilemaps.Tiles;
using UnityEditor;
using UnityEngine;

namespace FeedTheBaby.Tilemaps.Brushes
{
    [CustomPropertyDrawer(typeof(TerrainTileBrushDictionary))]
    public class TerrainTypeBrushDictionaryDrawer : SerializableDictionaryDrawer<TerrainType, GridBrushBase>
    {
        protected override SerializableKeyValueTemplate<TerrainType, GridBrushBase> GetTemplate()
        {
            return GetGenericTemplate<SerializableTerrainTypeBrushTemplate>();
        }

        internal class SerializableTerrainTypeBrushTemplate : SerializableKeyValueTemplate<TerrainType, GridBrushBase>
        {
        }
    }
}