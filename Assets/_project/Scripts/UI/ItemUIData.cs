using System;
using FeedTheBaby.Dictionary;
using UnityEngine;

#if UNITY_EDITOR
using FeedTheBaby.Editor.Dictionary;
#endif

namespace FeedTheBaby.UI
{
    [CreateAssetMenu(fileName = "ItemUI", menuName = "Feed The Baby/ItemUI", order = 0)]
    public class ItemUIData : ScriptableObject
    {
        public ItemSpriteDictionary itemSpriteDictionary = new ItemSpriteDictionary();

        public Sprite GetSprite(ItemType type)
        {
            return itemSpriteDictionary.dictionary.ContainsKey(type) ? itemSpriteDictionary.dictionary[type] : null;
        }
    }


    [Serializable]
    public class ItemSpriteDictionary : SerializableDictionary<ItemType, Sprite>
    {
    }
    
    #if UNITY_EDITOR
    
    [UnityEditor.CustomPropertyDrawer(typeof(ItemSpriteDictionary))]
    public class StringIntDictionaryDrawer : SerializableDictionaryDrawer<ItemType, Sprite>
    {
        protected override SerializableKeyValueTemplate<ItemType, Sprite> GetTemplate()
        {
            return GetGenericTemplate<SerializableItemSpriteUITemplate>();
        }
    }

    internal class SerializableItemSpriteUITemplate : SerializableKeyValueTemplate<ItemType, Sprite>
    {
    }
    
    #endif
}