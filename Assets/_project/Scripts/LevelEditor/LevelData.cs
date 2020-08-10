using System;
using FeedTheBaby.Tiles;
using UnityEngine;
using TerrainTile = FeedTheBaby.Tiles.Terrain.TerrainTile;

namespace FeedTheBaby.LevelEditor
{
    [CreateAssetMenu(fileName = "Level Data", menuName = "Feed The Baby/Level Data", order = 1)]
    public class LevelData : ScriptableObject
    {
        public ItemTier[] goals = null;
        public Vector3Int[] terrainPositions = null;
        public TerrainTile[] terrainTiles = null;
        public Vector3Int[] levelObjectPositions = null;
        public LevelObjectTile[] levelObjectTiles = null;
        public ItemAmount[] initialInventory = null;
        public HintData[] hints = null;
        public Vector2 playerStartPosition;
        public Vector2 babyStartPosition;
        public int fuelAmount = 0;
        public string levelName = "";
        public float levelTime;
        public float playerStartTime;
    }

    [Serializable]
    public struct HintData
    {
        public Vector2 position;
        public string text;
        public float duration;
        public bool showOnce;
        public Rect trigger;
    }
}