using System;
using FeedTheBaby.Tilemaps.Tiles;
using UnityEngine;

namespace FeedTheBaby.Tilemaps.Brushes
{
    [CreateAssetMenu(fileName = "Level Object Data", menuName = "Feed The Baby/Level Object Data", order = 0)]
    public class LevelObjectMap : ScriptableObject
    {
        [SerializeField] public LevelObjectDictionary dictionary = LevelObjectDictionary.New<LevelObjectDictionary>();

        public GameObject GetPrefab(LevelObjectType type)
        {
            return dictionary.dictionary.ContainsKey(type) ? dictionary.dictionary[type].prefab : null;
        }

        public LevelObjectTile GetTile(LevelObjectType type)
        {
            return dictionary.dictionary.ContainsKey(type) ? dictionary.dictionary[type].tile : null;
        }

        [Serializable]
        public struct LevelObjectData
        {
            [SerializeField] public LevelObjectTile tile;
            [SerializeField] public GameObject prefab;
        }
    }
}