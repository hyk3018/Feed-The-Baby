using System.Collections.Generic;
using FeedTheBaby.Tilemaps.Tiles;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;
using TerrainTile = FeedTheBaby.Tilemaps.Tiles.TerrainTile;

#if UNITY_EDITOR

namespace FeedTheBaby.Tilemaps.Brushes
{
    [CustomGridBrush(false, true, false, "Terrain Deco Brush")]
    [CreateAssetMenu(fileName = "New Terrain Decoration Brush", menuName = "Brushes/Terrain Decoration Brush",
        order = 0)]
    public class TerrainDecorationBrush : GridBrush
    {
        [SerializeField]
        TerrainTileBrushDictionary terrainTileDictionary = TerrainTileBrushDictionary.New<TerrainTileBrushDictionary>();

        Dictionary<TerrainType, GridBrushBase> TerrainTiles => terrainTileDictionary.dictionary;

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            base.Paint(gridLayout, brushTarget, position);
            if (brushTarget == null)
                return;

            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            var terrainMap = brushTarget.GetComponent<TerrainDecorationMap>();
            if (terrainMap == null)
                return;
            var terrainTile = terrainMap.TileMap().GetTile<TerrainTile>(position);
            if (terrainTile != null)
                terrainTileDictionary.dictionary[terrainTile.terrainType].Paint(gridLayout, brushTarget, position);
        }
    }
}

#endif