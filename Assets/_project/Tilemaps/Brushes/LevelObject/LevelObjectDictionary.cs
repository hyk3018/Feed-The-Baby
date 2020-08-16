using System;
using FeedTheBaby.Dictionary;
using FeedTheBaby.Tilemaps.Tiles;

namespace FeedTheBaby.Tilemaps.Brushes
{
    [Serializable]
    public class LevelObjectDictionary : SerializableDictionary<LevelObjectType, LevelObjectMap.LevelObjectData>
    {
    }
}