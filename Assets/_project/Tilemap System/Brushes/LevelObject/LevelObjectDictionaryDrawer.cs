using FeedTheBaby.Tiles;

#if UNITY_EDITOR
using FeedTheBaby.Editor.Dictionary;

namespace FeedTheBaby.Brushes
{
    [UnityEditor.CustomPropertyDrawer(typeof(LevelObjectDictionary))]
    public class
        LevelObjectDictionaryDrawer : SerializableDictionaryDrawer<LevelObjectType, LevelObjectMap.LevelObjectData>
    {
        protected override SerializableKeyValueTemplate<LevelObjectType, LevelObjectMap.LevelObjectData> GetTemplate()
        {
            return GetGenericTemplate<SerializableLevelObjectDictionaryTemplate>();
        }

        internal class
            SerializableLevelObjectDictionaryTemplate : SerializableKeyValueTemplate<LevelObjectType,
                LevelObjectMap.LevelObjectData>
        {
        }
    }
}

#endif