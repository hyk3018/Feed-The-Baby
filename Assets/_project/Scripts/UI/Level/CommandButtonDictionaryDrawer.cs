using FeedTheBaby.Commands;
using FeedTheBaby.Editor.Dictionary;
using UnityEngine;

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