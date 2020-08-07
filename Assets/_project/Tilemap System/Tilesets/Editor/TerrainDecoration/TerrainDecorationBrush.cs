using System;
using System.Collections.Generic;
using FeedTheBaby;
using FeedTheBaby.Dictionary;
using FeedTheBaby.Editor.Dictionary;
using FeedTheBaby.Tiles;
using FeedTheBaby.Tiles.Terrain;
using UnityEngine;
using UnityEngine.Tilemaps;
using TerrainMap = FeedTheBaby.Tiles.TerrainDecoration.TerrainMap;
using TerrainTile = FeedTheBaby.Tiles.Terrain.TerrainTile;

#if UNITY_EDITOR

namespace UnityEditor.Tilemaps
{
    [CustomGridBrush(false, true, false, "Terrain Deco Brush")]
    [CreateAssetMenu(fileName = "New Terrain Decoration Brush", menuName = "Brushes/Terrain Decoration Brush",
        order = 0)]
    public class TerrainDecorationBrush : GridBrush
    {
        [SerializeField]
        TerrainTileDictionary terrainTileDictionary = TerrainTileDictionary.New<TerrainTileDictionary>();

        Dictionary<TerrainType, GridBrushBase> TerrainTiles => terrainTileDictionary.dictionary;

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            base.Paint(gridLayout, brushTarget, position);
            if (brushTarget == null)
                return;

            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            var terrainMap = brushTarget.GetComponent<TerrainMap>();
            if (terrainMap == null)
                return;
            var terrainTile = terrainMap.TileMap().GetTile<TerrainTile>(position);
            if (terrainTile != null)
                terrainTileDictionary.dictionary[terrainTile.terrainType].Paint(gridLayout, brushTarget, position);
        }
    }

    [Serializable]
    public class TerrainTileDictionary : SerializableDictionary<TerrainType, GridBrushBase>
    {
    }

    [CustomPropertyDrawer(typeof(TerrainTileDictionary))]
    public class TerrainTypeBrushDictionaryDrawer : SerializableDictionaryDrawer<TerrainType, GridBrushBase>
    {
        protected override SerializableKeyValueTemplate<TerrainType, GridBrushBase> GetTemplate()
        {
            return GetGenericTemplate<SerializableTerrainTypeBrushTemplate>();
        }
    }

    internal class SerializableTerrainTypeBrushTemplate : SerializableKeyValueTemplate<TerrainType, GridBrushBase>
    {
    }
}

#endif