using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FeedTheBaby.Pathfinding;
using FeedTheBaby.Tilemaps.Brushes;
using FeedTheBaby.Tilemaps.Tiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using TerrainMap = FeedTheBaby.Tilemaps.Brushes.TerrainMap;
using TerrainTile = FeedTheBaby.Tilemaps.Tiles.TerrainTile;

namespace FeedTheBaby.LevelEditor
{
    public class LevelEditorWindow : EditorWindow
    {
        #region Level Data

        Tilemap _terrainTileMap = null;
        Tilemap _levelObjectsTileMap = null;
        Tilemap _obstructionsTileMap = null;
        string _levelName;
        Vector2 _playerPosition;
        Vector2 _babyPosition;
        ItemTier[] _goalTiers;
        List<ItemAmount> _inventoryOnLoad = null;
        int _fuelAmount;
        float _levelTime;
        float _playerStartTime;

        #endregion

        #region Helper Variables
        TerrainMap _terrainMap = null;
        LevelObjectMap _levelObjectMap = null;
        GameObject _player = null;
        GameObject _baby = null;
        Vector2 _scrollPos;
        bool[] _showTiers;
        ItemAmount[] _pendingAddGoal;
        ItemAmount _pendingAddInventory;
        GameObject _hintsContainer;

        #endregion

        #region UI Data

        Texture2D _plus;
        Texture2D _minus;
        GUIStyle _errorStyle;
        string _previousFolder = "";

        #endregion


        [MenuItem("Window/Feed The Baby/Level Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<LevelEditorWindow>(false, "Level Editor", true);
        }

        void OnEnable()
        {
            _errorStyle = new GUIStyle
                {normal = {textColor = Color.red}, alignment = TextAnchor.MiddleCenter};
            _plus = Resources.Load("plus") as Texture2D;
            _minus = Resources.Load("minus") as Texture2D;

            _goalTiers = new ItemTier[3];
            for (var i = 0; i < 3; i++) _goalTiers[i] = new ItemTier();

            _pendingAddGoal = new ItemAmount[3];
            for (var i = 0; i < 3; i++) _pendingAddGoal[i] = new ItemAmount(1);

            _inventoryOnLoad = new List<ItemAmount>();

            _showTiers = new bool[3] {true, true, true};

            _fuelAmount = 0;
            
            _previousFolder = "Assets/_project/Feed The Baby/Levels";
            
            _levelObjectMap = Resources.Load("Level Object Data") as LevelObjectMap;
            _terrainMap = Resources.Load("Terrain Data") as TerrainMap;
        }

        void OnInspectorUpdate()
        {
            if (_player && _player.transform.hasChanged)
            {
                _playerPosition = _player.transform.position;
                Repaint();
            }

            if (_baby && _baby.transform.hasChanged)
            {
                _babyPosition = _baby.transform.position;
                Repaint();
            }
        }

        void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);

            DrawButtons();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Level Name : ", EditorStyles.boldLabel);
            _levelName = EditorGUILayout.TextField(_levelName);
            GUILayout.EndHorizontal();
            DrawLineUI();

            DrawDataMapsUI();
            DrawLineUI();

            DrawTerrainTileMapUI();
            DrawLineUI();

            DrawLevelObjectsTileMapUI();
            DrawLineUI();

            DrawObstructionsTileMapUI();
            DrawLineUI();
            
            DrawTimeUI();
            DrawLineUI();

            DrawPlayerUI();
            DrawLineUI();

            DrawInventoryUI();
            DrawLineUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Fuel Amount : ", EditorStyles.boldLabel);
            _fuelAmount = EditorGUILayout.IntField(_fuelAmount);
            GUILayout.EndHorizontal();
            DrawLineUI();

            DrawBabyUI();
            DrawLineUI();

            DrawGoalsUI();
            DrawLineUI();

            DrawHintsUI();
            DrawLineUI();

            GUILayout.EndScrollView();
        }
        
        #region Loading

        void NewLevel()
        {
            if (EditorUtility.DisplayDialog("New Level",
                "Do you want to clear the level and start anew? Be sure to save any levels you would like" +
                "before clearing.",
                "Yes, clear", "No"))
            {
                LevelData defaultLevel = Resources.Load<LevelData>("default");
                if (defaultLevel)
                    LoadLevel(defaultLevel);
                else
                {
                    ClearLevel();
                }
            }
        }

        void ClearLevel()
        {
            _levelName = "";
            _player.transform.position = new Vector2(-1.5f, 0);
            _baby.transform.position = new Vector3(1.5f, 0);
            _inventoryOnLoad = new List<ItemAmount>();
            _levelTime = 0;
            _playerStartTime = 0;

            for (var i = 0; i < _goalTiers.Length; i++) _goalTiers[i] = new ItemTier();

            _levelObjectsTileMap.ClearAllTiles();
            var children = _levelObjectsTileMap.transform.Cast<Transform>().ToList();
            foreach (var child in children)
                DestroyImmediate(child.gameObject);

            foreach (Transform child in _hintsContainer.transform)
                DestroyImmediate(child.gameObject);
        }

        void LoadLevelFromPath()
        {
            var filePath = EditorUtility.OpenFilePanel("Open Level File", _previousFolder, "asset");
            if (filePath.Length != 0)
            {
                var relativePath = FileUtil.GetProjectRelativePath(filePath);
                var levelData = AssetDatabase.LoadAssetAtPath<LevelData>(relativePath);

                LoadLevel(levelData);
            }
        }

        void LoadLevel(LevelData levelData)
        {
            _levelName = levelData.levelName;
            _playerPosition = levelData.playerStartPosition;
            _player.transform.position = _playerPosition;
            _babyPosition = levelData.babyStartPosition;
            _baby.transform.position = _babyPosition;
            _inventoryOnLoad = levelData.initialInventory.ToList();
            _levelTime = levelData.levelTime;
            _playerStartTime = levelData.playerStartTime;
            _fuelAmount = levelData.fuelAmount;
            
            Array.Resize(ref _goalTiers, levelData.goals.Length);
            for (var i = 0; i < levelData.goals.Length; i++) _goalTiers[i] = ItemTier.Copy(levelData.goals[i]);

            // Load ground and obstructions
            _terrainTileMap.ClearAllTiles();
            _terrainTileMap.SetTiles(levelData.terrainPositions, levelData.terrainTiles);
            _obstructionsTileMap.ClearAllTiles();
            _obstructionsTileMap.SetTiles(levelData.obstructionPositions, levelData.obstructionTiles);

            LoadLevelObjects(levelData);

            LoadHints(levelData);
        }

        void LoadLevelObjects(LevelData levelData)
        {
            // Set level object tiles
            // Then instantiate game objects after destroying all children

            _levelObjectsTileMap.ClearAllTiles();
            _levelObjectsTileMap.SetTiles(levelData.levelObjectPositions, levelData.levelObjectTiles);

            var children = _levelObjectsTileMap.transform.Cast<Transform>().ToList();
            foreach (var child in children)
                DestroyImmediate(child.gameObject);

            for (var i = 0; i < levelData.levelObjectPositions.Length; i++)
            {
                InstantiateLevelObjectPrefabInCell(_levelObjectsTileMap.layoutGrid, _levelObjectsTileMap.gameObject,
                    levelData.levelObjectPositions[i],
                    _levelObjectMap.GetPrefab(levelData.levelObjectTiles[i].levelObjectType));
            }
        }

        void LoadHints(LevelData levelData)
        {
            var hints = _hintsContainer.transform.Cast<Transform>().ToList();
            foreach (Transform hint in hints)
                DestroyImmediate(hint.gameObject);

            if (levelData.hints != null)
            {
                foreach (HintData hint in levelData.hints)
                {
                    GameObject hintPrefab = Resources.Load<GameObject>("Hint Editor");
                    GameObject hintObject = Instantiate(hintPrefab, _hintsContainer.transform);
                    hintObject.GetComponent<Hint>().LoadHint(hint);
                }
            }
        }

        #endregion

        #region Saving

        void SaveAllLevels()
        {
            LevelData[] levelDatas = GetAllInstances<LevelData>();
            foreach (LevelData levelData in levelDatas)
            {
                LoadLevel(levelData);
                SaveToLevelAsset(levelData);
                AssetDatabase.Refresh();
                EditorUtility.SetDirty(levelData);
                AssetDatabase.SaveAssets();
            }
        }
        
        static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:"+ typeof(T).Name);  //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for(int i =0;i<guids.Length;i++)         //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }
 
            return a;
 
        }
        
        void SaveLevel()
        {
            var savePath = EditorUtility.SaveFolderPanel("Save Level Data", _previousFolder, "");
            if (savePath.Length != 0)
            {
                if (_levelName.Length == 0)
                    _levelName = "level";

                _previousFolder = savePath;

                if (GetNewFileNameFromFolderPath(ref savePath))
                {
                    var levelAsset = CreateInstance<LevelData>();
                    SaveToLevelAsset(levelAsset);
                    AssetDatabase.CreateAsset(levelAsset, savePath);
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    var levelAsset = AssetDatabase.LoadAssetAtPath<LevelData>(savePath);
                    SaveToLevelAsset(levelAsset);
                    AssetDatabase.Refresh();
                    EditorUtility.SetDirty(levelAsset);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        bool GetNewFileNameFromFolderPath(ref string savePath)
        {
            if (File.Exists($"{savePath}/{_levelName}.asset"))
                if (EditorUtility.DisplayDialog("Saving file",
                    "Would you like to overwrite any existing levels of the same name?",
                    "Yes, overwrite", "No, create new"))
                {
                    savePath = FileUtil
                        .GetProjectRelativePath($"{savePath}/{_levelName}.asset");
                    return false;
                }

            string currentPath;
            var i = -1;
            do
            {
                var currentName = i == -1 ? _levelName : _levelName + i;
                currentPath = $"{savePath}/{currentName}.asset";
                i++;
            } while (File.Exists(currentPath));

            savePath = FileUtil.GetProjectRelativePath(currentPath);
            return true;
        }

        void SaveToLevelAsset(LevelData levelAsset)
        {
            levelAsset.levelName = _levelName;
            levelAsset.goals = ItemTier.CopyTiers(_goalTiers);
            levelAsset.playerStartPosition = _playerPosition;
            levelAsset.babyStartPosition = _babyPosition;
            levelAsset.levelTime = _levelTime;
            levelAsset.playerStartTime = _playerStartTime;

            levelAsset.initialInventory = _inventoryOnLoad.ToArray();
            levelAsset.fuelAmount = _fuelAmount;

            SaveTerrain(levelAsset);
            SaveObstructions(levelAsset);
            SaveLevelObjects(levelAsset);
            SaveHints(levelAsset);
        }
        
        void SaveTerrain(LevelData levelAsset)
        {
            var terrainPositions = new List<Vector3Int>();
            var terrainTypes = new List<TerrainTile>();
            _terrainTileMap.CompressBounds();
            var bounds = _terrainTileMap.cellBounds;

            foreach (var pos in bounds.allPositionsWithin)
            {
                if (_terrainTileMap.GetTile(pos) is TerrainTile terrainTile)
                {
                    terrainPositions.Add(pos);
                    terrainTypes.Add(terrainTile);
                }
            }

            levelAsset.terrainPositions = terrainPositions.ToArray();
            levelAsset.terrainTiles = terrainTypes.ToArray();
        }
        
        void SaveObstructions(LevelData levelAsset)
        {
            var obstructionPositions = new List<Vector3Int>();
            var obstructionTypes = new List<TileBase>();
            _obstructionsTileMap.CompressBounds();
            var bounds = _obstructionsTileMap.cellBounds;

            foreach (var pos in bounds.allPositionsWithin)
            {
                if (_obstructionsTileMap.GetTile(pos) is TileBase obstructionTile)
                {
                    obstructionPositions.Add(pos);
                    obstructionTypes.Add(obstructionTile);
                }
            }

            levelAsset.obstructionPositions = obstructionPositions.ToArray();
            levelAsset.obstructionTiles = obstructionTypes.ToArray();
        }

        void SaveHints(LevelData levelAsset)
        {
            var hintData = new List<HintData>();
            foreach (Transform child in _hintsContainer.transform)
            {
                Hint hint = child.GetComponent<Hint>();
                hintData.Add(hint.AsHintData());
            }

            levelAsset.hints = hintData.ToArray();
        }

        // Save tile maps to arrays
        void SaveLevelObjects(LevelData levelAsset)
        {
            var levelObjectPositions = new List<Vector3Int>();
            var levelObjectTypes = new List<LevelObjectTile>();
            _levelObjectsTileMap.CompressBounds();
            var bounds = _levelObjectsTileMap.cellBounds;

            foreach (var pos in bounds.allPositionsWithin)
                if (_levelObjectsTileMap.GetTile(pos) is LevelObjectTile levelObjectTile)
                {
                    levelObjectPositions.Add(pos);
                    levelObjectTypes.Add(levelObjectTile);
                }

            levelAsset.levelObjectPositions = levelObjectPositions.ToArray();
            levelAsset.levelObjectTiles = levelObjectTypes.ToArray();
        }
        
        #endregion
        
        #region UI Functions
        
        void DrawDataMapsUI()
        {
            _terrainMap = EditorGUILayout.ObjectField(_terrainMap, typeof(TerrainMap), true) as TerrainMap;
            _levelObjectMap =
                EditorGUILayout.ObjectField(_levelObjectMap, typeof(LevelObjectMap), true) as LevelObjectMap;

            if (!_terrainMap || !_levelObjectMap)
                GUILayout.Label("Missing terrain and level object data maps.", _errorStyle);
        }

        void DrawTerrainTileMapUI()
        {
            GUILayout.Label("Terrain Tile Map : ", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            var terrainObject = GameObject.FindWithTag("Terrain");

            if (terrainObject)
            {
                _terrainTileMap = terrainObject.GetComponent<Tilemap>();
                GUI.enabled = false;
                EditorGUILayout.ObjectField(_terrainTileMap, typeof(Tilemap), true);
                GUI.enabled = true;
            }
            else
            {
                GUILayout.Label("No GameObject with tag \"Terrain\" found.", _errorStyle);
            }

            EditorGUILayout.EndHorizontal();
        }

        void DrawLevelObjectsTileMapUI()
        {
            GUILayout.Label("Level Objects Tile Map : ", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            var levelObjectsObject = GameObject.FindWithTag("LevelObjects");
            if (levelObjectsObject)
            {
                _levelObjectsTileMap = levelObjectsObject.GetComponent<Tilemap>();
                GUI.enabled = false;
                EditorGUILayout.ObjectField(_levelObjectsTileMap, typeof(Tilemap), true);
                GUI.enabled = true;
            }
            else
            {
                GUILayout.Label("No GameObject with tag \"LevelObjects\" found.", _errorStyle);
            }

            EditorGUILayout.EndHorizontal();
        }

        void DrawObstructionsTileMapUI()
        {
            GUILayout.Label("Obstructions Tile Map : ", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            var obstructionsObject = GameObject.FindWithTag("Obstructions");
            if (obstructionsObject)
            {
                _obstructionsTileMap = obstructionsObject.GetComponent<Tilemap>();
                GUI.enabled = false;
                EditorGUILayout.ObjectField(_obstructionsTileMap, typeof(Tilemap), true);
                GUI.enabled = true;
            }
            else
            {
                GUILayout.Label("No GameObject with tag \"Obstructions\" found.", _errorStyle);
            }
            
            EditorGUILayout.EndHorizontal();
        }

        void DrawPlayerUI()
        {
            GUILayout.Label("Player : ", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            _player = GameObject.FindWithTag("Player");
            EditorGUILayout.EndHorizontal();

            if (_player && _player.CompareTag("Player"))
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField(_player, typeof(GameObject), true);
                GUI.enabled = true;
                EditorGUI.BeginChangeCheck();
                _playerPosition = EditorGUILayout.Vector2Field("Player Position :", _playerPosition);
                if (EditorGUI.EndChangeCheck())
                    _player.transform.position = _playerPosition;
            }
            else
            {
                _player = null;
                GUILayout.Label("Missing player", _errorStyle);
            }
        }

        void DrawBabyUI()
        {
            GUILayout.Label("Baby : ", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            _baby = GameObject.FindWithTag("Baby");
            EditorGUILayout.EndHorizontal();

            if (_baby && _baby.CompareTag("Baby"))
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField(_baby, typeof(GameObject), true);
                GUI.enabled = true;
                EditorGUI.BeginChangeCheck();
                _babyPosition = EditorGUILayout.Vector2Field("Baby Position : ", _babyPosition);
                if (EditorGUI.EndChangeCheck())
                    _baby.transform.position = _babyPosition;
            }
            else
            {
                _baby = null;
                GUILayout.Label("Missing baby", _errorStyle);
            }
        }

        void DrawGoalsUI()
        {
            GUILayout.Label("Goals", EditorStyles.boldLabel);
            for (var i = 0; i < 3; i++) DrawTierUI(i);
        }

        void DrawTierUI(int i)
        {
            _showTiers[i] = EditorGUILayout.Foldout(_showTiers[i], "Tier " + (i + 1), EditorStyles.foldout);
            if (_showTiers[i])
            {
                for (var j = 0; j < _goalTiers[i].items.Count; j++)
                {
                    GUILayout.BeginHorizontal();

                    var item = _goalTiers[i].items[j];

                    EditorGUI.BeginChangeCheck();
                    GUILayout.Label("Item Amount : ");
                    item.type = (ItemType) EditorGUILayout.EnumPopup(item.type);
                    item.amount = EditorGUILayout.IntField(item.amount);

                    if (GUILayout.Button(_minus))
                    {
                        EditorGUI.EndChangeCheck();
                        _goalTiers[i].RemoveItem(j);
                        j--;
                    }
                    else
                    {
                        if (EditorGUI.EndChangeCheck()) _goalTiers[i].items[j] = item;
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();
                GUILayout.Label("Add Item Amount : ");
                var pending = _pendingAddGoal[i];
                pending.type = (ItemType) EditorGUILayout.EnumPopup(pending.type);
                pending.amount = EditorGUILayout.IntField(pending.amount);

                if (EditorGUI.EndChangeCheck()) _pendingAddGoal[i] = pending;

                if (GUILayout.Button(_plus)) _goalTiers[i].AddItem(_pendingAddGoal[i]);
                GUILayout.EndHorizontal();
            }
        }

        void DrawInventoryUI()
        {
            GUILayout.Label("Starting Inventory : ", EditorStyles.boldLabel);
            for (var j = 0; j < _inventoryOnLoad.Count; j++)
            {
                GUILayout.BeginHorizontal();

                var item = _inventoryOnLoad[j];

                EditorGUI.BeginChangeCheck();
                GUILayout.Label("Item Amount : ");
                item.type = (ItemType) EditorGUILayout.EnumPopup(item.type);
                item.amount = EditorGUILayout.IntField(item.amount);

                if (GUILayout.Button(_minus))
                {
                    EditorGUI.EndChangeCheck();
                    _inventoryOnLoad.RemoveAt(j);
                    j--;
                }
                else
                {
                    if (EditorGUI.EndChangeCheck()) _inventoryOnLoad[j] = item;
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            GUILayout.Label("Add Item Amount : ");
            var pending = _pendingAddInventory;
            pending.type = (ItemType) EditorGUILayout.EnumPopup(pending.type);
            pending.amount = EditorGUILayout.IntField(pending.amount);

            if (EditorGUI.EndChangeCheck()) _pendingAddInventory = pending;

            if (GUILayout.Button(_plus)) ItemAmount.Add(_inventoryOnLoad, _pendingAddInventory);
            GUILayout.EndHorizontal();
        }

        void DrawHintsUI()
        {
            GUILayout.Label("Hints : ", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            _hintsContainer = GameObject.FindWithTag("Hints");
            EditorGUILayout.EndHorizontal();

            if (_hintsContainer && _hintsContainer.CompareTag("Hints"))
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField(_hintsContainer, typeof(GameObject), true);
                GUI.enabled = true;

                for (int i = 0; i < _hintsContainer.transform.childCount; i++)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    GUILayout.Label($"Hint {i + 1} : ");
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField(_hintsContainer.transform.GetChild(i), typeof(GameObject), true);
                    GUI.enabled = true;

                    if (GUILayout.Button(_minus))
                    {
                        DestroyImmediate(_hintsContainer.transform.GetChild(i).gameObject);
                        i--;
                    }

                    GUILayout.EndHorizontal();
                }
                
                GUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();
                GUILayout.Label("Add Another Hint : ");

                if (GUILayout.Button(_plus))
                {
                    GameObject hintEditorPrefab = Resources.Load<GameObject>("Hint Editor");
                    Instantiate(hintEditorPrefab, _hintsContainer.transform);
                }
                
                GUILayout.EndHorizontal();
            }
            else
            {
                _hintsContainer = null;
                GUILayout.Label("Missing hints container", _errorStyle);
            }
        }
        

        void DrawTimeUI()
        {
            DrawFloatUI("Level Time : ", ref _levelTime);
            DrawLineUI();
            DrawFloatUI("Player Start Time : ", ref _playerStartTime);
            DrawLineUI();
        }

        static void DrawFloatUI(string labelText, ref float floatVariable)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(labelText, EditorStyles.boldLabel);
            floatVariable = EditorGUILayout.FloatField(floatVariable);
            GUILayout.EndHorizontal();
        }

        void DrawButtons()
        {
            if (!ValidFields()) GUI.enabled = false;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Level")) LoadLevelFromPath();

            if (GUILayout.Button("Save Level")) SaveLevel();

            if (GUILayout.Button("New Level")) NewLevel();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save All Levels")) SaveAllLevels();
            EditorGUILayout.EndHorizontal();

            GUI.enabled = true;
        }
        
        #endregion

        #region Utility Functions
        
        bool ValidFields()
        {
            return _baby && _player && _terrainTileMap && _levelObjectsTileMap && _terrainMap && _levelObjectMap
                && _hintsContainer;
        }

        static void DrawLineUI(Color? color = null, int thickness = 2, int padding = 10)
        {
            var r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            color = color ?? Color.white;
            EditorGUI.DrawRect(r, color.Value);
        }

        static void InstantiateLevelObjectPrefabInCell(GridLayout grid, GameObject brushTarget,
            Vector3Int position, GameObject prefab)
        {
            var instance = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
            if (instance != null)
            {
                Undo.MoveGameObjectToScene(instance, brushTarget.scene, "Paint Prefabs");
                Undo.RegisterCreatedObjectUndo((Object) instance, "Paint Prefabs");
                instance.transform.SetParent(brushTarget.transform);
                instance.transform.position =
                    grid.LocalToWorld(grid.CellToLocalInterpolated(position + new Vector3(0.5f, 0.5f, 0.5f)));
            }
        }
        
        #endregion
    }
}