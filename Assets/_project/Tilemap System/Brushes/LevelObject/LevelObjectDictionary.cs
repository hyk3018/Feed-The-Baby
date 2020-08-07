using System;
using FeedTheBaby.Dictionary;
using FeedTheBaby.Tiles;

namespace FeedTheBaby.Brushes
{
    [Serializable]
    public class LevelObjectDictionary : SerializableDictionary<LevelObjectType, LevelObjectMap.LevelObjectData>
    {
    }
}