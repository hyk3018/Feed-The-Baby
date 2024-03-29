﻿using FeedTheBaby.Tilemaps.Tiles;
using FeedTheBaby.Tilemaps.Tiles.Terrain;
using UnityEngine;

#if UNITY_EDITOR
namespace FeedTheBaby.Tilemaps.Brushes
{
    [CreateAssetMenu(fileName = "Terrain Data", menuName = "Feed The Baby/Terrain Data", order = 0)]
    public class TerrainMap : ScriptableObject
    {
        [SerializeField] public TerrainTileDictionary dictionary = TerrainTileDictionary.New<TerrainTileDictionary>();

        public TerrainTile GetTile(TerrainType type)
        {
            return dictionary.dictionary.ContainsKey(type) ? dictionary.dictionary[type] : null;
        }
    }
}

#endif