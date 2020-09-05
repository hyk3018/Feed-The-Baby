using System;
using FeedTheBaby.Dictionary;
using FeedTheBaby.Tilemaps.Tiles;
using FeedTheBaby.Tilemaps.Tiles.Terrain;
using UnityEngine;

namespace FeedTheBaby.Tilemaps.Brushes
{
    [Serializable]
    public class TerrainTileBrushDictionary : SerializableDictionary<TerrainType, GridBrushBase>
    {
    }
}