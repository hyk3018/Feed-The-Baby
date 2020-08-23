﻿using UnityEngine.Tilemaps;

namespace FeedTheBaby.Tilemaps.Tiles
{
    public enum TerrainType
    {
        Grassland,
        Desert,
        PineTree
    }

    [UnityEngine.CreateAssetMenu(fileName = "Terrain Tile", menuName = "Tiles/Terrain Tile", order = 0)]
    public class TerrainTile : Tile
    {
        public TerrainType terrainType;
    }
}