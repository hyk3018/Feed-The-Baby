using System;
using FeedTheBaby.Dictionary;
using FeedTheBaby.Tilemaps.Tiles;
using FeedTheBaby.Tilemaps.Tiles.Terrain;

namespace FeedTheBaby.Tilemaps.Brushes
{
    [Serializable]
    public class TerrainTileDictionary : SerializableDictionary<TerrainType, TerrainTile>
    {
    }
}