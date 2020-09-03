using FeedTheBaby.Commands;
using UnityEngine;

#if UNITY_EDITOR

using FeedTheBaby.Editor.Dictionary;

namespace FeedTheBaby.UI
{
    [UnityEditor.CustomPropertyDrawer(typeof(CommandButtonDictionary))]
    public class CommandButtonDictionaryDrawer : SerializableDictionaryDrawer<CommandType, GameObject>
    {
        protected override SerializableKeyValueTemplate<CommandType, GameObject> GetTemplate()
        {
            return GetGenericTemplate<SerializableCommandButtonDictionaryTemplate>();
        }

        internal class
            SerializableCommandButtonDictionaryTemplate : SerializableKeyValueTemplate<CommandType,
                GameObject>
        {
        }
    }
}

#endif