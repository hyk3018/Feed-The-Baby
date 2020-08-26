using System;
using System.Linq;
using FeedTheBaby.Game;
using FeedTheBaby.GameData;
using FeedTheBaby.LevelEditor;
using FeedTheBaby.LevelObjects;
using FeedTheBaby.Pathfinding;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FeedTheBaby
{
    [RequireComponent(typeof(Timer))]
    public class LevelManager : MonoBehaviour
    {
        static LevelManager _instance;

        [SerializeField] GameObject player = null;
        [SerializeField] GameObject baby = null;
        [SerializeField] Tilemap terrainTileMap = null;
        [SerializeField] Tilemap levelObjectsTileMap = null;
        [SerializeField] Tilemap obstructionsTileMap = null;
        [SerializeField] GameObject hints = null;

        public int currentLevel;
        public LevelData currentLevelData;
        public NavGrid navigationGrid;

        Goals _goals;
        Timer _timer;
        public Action<Goals> LevelStart;
        public Action GameEnd;
        public Action EndWithStarsUncollected;

        public static LevelManager Instance => _instance;

        void Awake()
        {
            if (_instance == null || _instance != this)
            {
                _instance = this;
                _timer = GetComponent<Timer>();
                _goals = GetComponent<Goals>();
                LoadLevel();
            }
        }

        void Start()
        {
            LevelStart(_goals);
            _timer.StartCount(currentLevelData.levelTime);
            _goals.FinalTierFilled += OnGameEnd;
            _goals.FinalTierFilled += GameEnd;
            _timer.TimerEnd += CheckGameEnd;
            GameEnd += OnGameEnd;
        }

        void CheckGameEnd(Timer timer)
        {
            AudioSource.PlayClipAtPoint(baby.GetComponent<Baby>().cryingSound, player.transform.position);
            EndWithStarsUncollected();
            OnGameEnd();
            GameEnd();
        }

        void OnGameEnd()
        {
            if (_goals.CollectedStars > 0) DataService.Instance.UnlockNextLevel(currentLevel);
        }

        void LoadLevel()
        {
            currentLevel = DataService.Instance.GetCurrentLevel();
            currentLevelData = DataService.Instance.GetLevels()[currentLevel];

            player.transform.position = currentLevelData.playerStartPosition;
            baby.transform.position = currentLevelData.babyStartPosition;

            terrainTileMap.ClearAllTiles();
            terrainTileMap.SetTiles(currentLevelData.terrainPositions, currentLevelData.terrainTiles);
            obstructionsTileMap.ClearAllTiles();
            obstructionsTileMap.SetTiles(currentLevelData.obstructionPositions, currentLevelData.obstructionTiles);

            // Set level object tiles
            // Then instantiate gameobjects after destroying all children

            levelObjectsTileMap.SetTiles(currentLevelData.levelObjectPositions, currentLevelData.levelObjectTiles);
            
            navigationGrid = new NavGrid(terrainTileMap, levelObjectsTileMap, obstructionsTileMap);
            navigationGrid.CalculateGridNavigation();
            
            var children = levelObjectsTileMap.transform.Cast<Transform>().ToList();
            foreach (var child in children)
                DestroyImmediate(child.gameObject);

            for (var i = 0; i < currentLevelData.levelObjectPositions.Length; i++)
            {
                Vector3Int levelObjectPosition = currentLevelData.levelObjectPositions[i];
                var pos = levelObjectPosition;
                InstantiateLevelObjectPrefabInCell(levelObjectsTileMap.layoutGrid, levelObjectsTileMap.gameObject,
                    levelObjectPosition,
                    DataService.Instance.GetLevelObjectMap()
                        .GetPrefab(currentLevelData.levelObjectTiles[i].levelObjectType),
                    () =>
                    {
                        levelObjectsTileMap.SetTile(levelObjectPosition, null);
                        navigationGrid.CalculateCellNavigation(levelObjectPosition);
                    });
            }

            children = hints.transform.Cast<Transform>().ToList();
            foreach (var child in children)
                DestroyImmediate(child.gameObject);

            if (currentLevelData.hints != null)
            {
                foreach (var hint in currentLevelData.hints)
                {
                    GameObject hintPrefab = Resources.Load<GameObject>("Hint Editor");
                    GameObject hintObject = Instantiate(hintPrefab, hints.transform);
                    hintObject.GetComponent<Hint>().LoadHint(hint);
                }
            }
        }

        static void InstantiateLevelObjectPrefabInCell(GridLayout grid, GameObject brushTarget, Vector3Int position,
            GameObject prefab, Action clearTile)
        {
            var instance = Instantiate(prefab);
            if (instance != null)
            {
                LevelObject levelObject = instance.GetComponent<LevelObject>();
                if (levelObject == null)
                {
                    levelObject = instance.AddComponent<LevelObject>();
                }
                
                levelObject.destroyTile = clearTile;
                
                instance.transform.SetParent(brushTarget.transform);
                instance.transform.position =
                    grid.LocalToWorld(grid.CellToLocalInterpolated(position + new Vector3(0.5f, 0.55f, 0.5f)));
            }
        }
    }
}