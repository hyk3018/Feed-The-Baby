using System;
using FeedTheBaby.Dictionary;
using FeedTheBaby.Tiles.Terrain;

namespace FeedTheBaby.Editor.Brushes
{
    [Serializable]
    public class TerrainTileDictionary : SerializableDictionary<TerrainType, TerrainTile>
    {
    }
}