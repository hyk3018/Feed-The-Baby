using System;
using System.Linq;
using FeedTheBaby.GameData;
using FeedTheBaby.LevelEditor;
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
        [SerializeField] Tilemap terrainTilemap = null;
        [SerializeField] Tilemap levelObjectsTilemap = null;
        [SerializeField] GameObject hints = null;

        public int currentLevel;
        public LevelData currentLevelData;

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

            // _inventoryOnLoad = levelData.initialInventory.ToList();

            terrainTilemap.SetTiles(currentLevelData.terrainPositions, currentLevelData.terrainTiles);

            // Set level object tiles
            // Then instantiate gameobjects after destroying all children

            levelObjectsTilemap.SetTiles(currentLevelData.levelObjectPositions, currentLevelData.levelObjectTiles);

            var children = levelObjectsTilemap.transform.Cast<Transform>().ToList();
            foreach (var child in children)
                DestroyImmediate(child.gameObject);

            for (var i = 0; i < currentLevelData.levelObjectPositions.Length; i++)
            {
                var pos = currentLevelData.levelObjectPositions[i];
                InstantiateLevelObjectPrefabInCell(levelObjectsTilemap.layoutGrid, levelObjectsTilemap.gameObject,
                    currentLevelData.levelObjectPositions[i],
                    DataService.Instance.GetLevelObjectMap()
                        .GetPrefab(currentLevelData.levelObjectTiles[i].levelObjectType));
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

            // _fuelAmount = levelData.fuelAmount;
        }

        static void InstantiateLevelObjectPrefabInCell(GridLayout grid, GameObject brushTarget, Vector3Int position,
            GameObject prefab)
        {
            var instance = Instantiate(prefab);
            if (instance != null)
            {
                instance.transform.SetParent(brushTarget.transform);
                instance.transform.position =
                    grid.LocalToWorld(grid.CellToLocalInterpolated(position + new Vector3(0.5f, 0.5f, 0.5f)));
            }
        }
    }
}