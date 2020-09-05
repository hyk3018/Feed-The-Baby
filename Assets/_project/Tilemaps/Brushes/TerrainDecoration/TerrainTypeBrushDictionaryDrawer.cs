using FeedTheBaby.Tilemaps.Tiles;
using FeedTheBaby.Tilemaps.Tiles.Terrain;
using UnityEngine;

#if UNITY_EDITOR

using FeedTheBaby.Editor.Dictionary;
using UnityEditor;

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

#endif