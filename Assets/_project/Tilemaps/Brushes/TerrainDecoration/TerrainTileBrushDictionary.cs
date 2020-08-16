using System;
using FeedTheBaby.Dictionary;
using FeedTheBaby.Tilemaps.Tiles;
using UnityEngine;

namespace FeedTheBaby.Tilemaps.Brushes
{
    [Serializable]
    public class TerrainTileBrushDictionary : SerializableDictionary<TerrainType, GridBrushBase>
    {
    }
}