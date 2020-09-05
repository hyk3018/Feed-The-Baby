using FeedTheBaby.Pathfinding;
using FeedTheBaby.Tilemaps.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;
using TerrainTile = FeedTheBaby.Tilemaps.Tiles.TerrainTile;

namespace FeedTheBaby.LevelEditor
{
    [CreateAssetMenu(fileName = "Level Data", menuName = "Feed The Baby/Level Data", order = 1)]
    public class LevelData : ScriptableObject
    {
        public ItemTier[] goals = null;
        
        public Vector3Int[] terrainPositions = null;
        public TileBase[] terrainTiles = null;
        public Vector3Int[] obstructionPositions = null;
        public TileBase[] obstructionTiles = null;
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
}