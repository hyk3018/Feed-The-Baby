﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FeedTheBaby.Brushes;
using FeedTheBaby.Editor.Brushes;
using FeedTheBaby.Tiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using TerrainTile = FeedTheBaby.Tiles.Terrain.TerrainTile;

namespace FeedTheBaby.LevelEditor
{
    public class LevelEditorWindow : EditorWindow
    {
        #region Level Data

        Tilemap _terrainTileMap = null;
        Tilemap _levelObjectsTileMap = null;
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

        #endregion

        #region UI Data

        Texture2D _plus;
        Texture2D _minus;

        string previousFolder = "";

        #endregion

        #region Editor Serialization

        SerializedObject _serializedObject;

        #endregion

        GUIStyle _errorStyle;

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

            DrawTerrainTilemapUI();
            DrawLineUI();

            DrawLevelObjectsTilemapUI();
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

            GUILayout.EndScrollView();
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

        // static void DrawVariableUI<T>(string labelText, ref T variable)
        // {
        //     GUILayout.BeginHorizontal();
        //     GUILayout.Label(labelText, EditorStyles.boldLabel);
        //     
        //     switch (variable)
        //     {
        //         case int i:
        //         {
        //             
        //             variable = EditorGUILayout.IntField(i, EditorStyles.boldLabel);
        //             break;
        //         }
        //     }
        //     GUILayout.EndHorizontal();
        // }

        void DrawButtons()
        {
            if (!ValidFields()) GUI.enabled = false;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Level")) LoadLevel();

            if (GUILayout.Button("Save Level")) SaveLevel();

            if (GUILayout.Button("New Level")) NewLevel();
            EditorGUILayout.EndHorizontal();

            GUI.enabled = true;
        }

        void NewLevel()
        {
            if (EditorUtility.DisplayDialog("New Level",
                "Do you want to clear the level and start anew? Be sure to save any levels you would like" +
                "before clearing.",
                "Yes, clear", "No"))
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
            }
        }

        void LoadLevel()
        {
            var filePath = EditorUtility.OpenFilePanel("Open Level File", previousFolder, "asset");
            if (filePath.Length != 0)
            {
                var relativePath = FileUtil.GetProjectRelativePath(filePath);
                var levelData = AssetDatabase.LoadAssetAtPath<LevelData>(relativePath);

                _levelName = levelData.levelName;
                _player.transform.position = levelData.playerStartPosition;
                _baby.transform.position = levelData.babyStartPosition;
                _inventoryOnLoad = levelData.initialInventory.ToList();
                _levelTime = levelData.levelTime;
                _playerStartTime = levelData.playerStartTime;

                Array.Resize(ref _goalTiers, levelData.goals.Length);
                for (var i = 0; i < levelData.goals.Length; i++) _goalTiers[i] = ItemTier.Copy(levelData.goals[i]);

                _terrainTileMap.SetTiles(levelData.terrainPositions, levelData.terrainTiles);

                // Set level object tiles
                // Then instantiate gameobjects after destroying all children

                _levelObjectsTileMap.ClearAllTiles();
                _levelObjectsTileMap.SetTiles(levelData.levelObjectPositions, levelData.levelObjectTiles);

                var children = _levelObjectsTileMap.transform.Cast<Transform>().ToList();
                foreach (var child in children)
                    DestroyImmediate(child.gameObject);

                for (var i = 0; i < levelData.levelObjectPositions.Length; i++)
                {
                    var pos = levelData.levelObjectPositions[i];
                    InstantiateLevelObjectPrefabInCell(_levelObjectsTileMap.layoutGrid, _levelObjectsTileMap.gameObject,
                        levelData.levelObjectPositions[i],
                        _levelObjectMap.GetPrefab(levelData.levelObjectTiles[i].levelObjectType));
                }

                _fuelAmount = levelData.fuelAmount;
            }
        }

        void SaveLevel()
        {
            var savePath = EditorUtility.SaveFolderPanel("Save Level Data", previousFolder, "");
            if (savePath.Length != 0)
            {
                if (_levelName.Length == 0)
                    _levelName = "level";

                previousFolder = savePath;

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
                    // EditorUtility.SetDirty(levelAsset);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
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
            SaveLevelObjects(levelAsset);
        }

        // Save tilemaps to arrays
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

        void SaveTerrain(LevelData levelAsset)
        {
            var terrainPositions = new List<Vector3Int>();
            var terrainTypes = new List<TerrainTile>();
            _terrainTileMap.CompressBounds();
            var bounds = _terrainTileMap.cellBounds;

            foreach (var pos in bounds.allPositionsWithin)
                if (_terrainTileMap.GetTile(pos) is TerrainTile terrainTile)
                {
                    terrainPositions.Add(pos);
                    terrainTypes.Add(terrainTile);
                }

            levelAsset.terrainPositions = terrainPositions.ToArray();
            levelAsset.terrainTiles = terrainTypes.ToArray();
        }


        void DrawDataMapsUI()
        {
            _terrainMap = EditorGUILayout.ObjectField(_terrainMap, typeof(TerrainMap), true) as TerrainMap;
            _levelObjectMap =
                EditorGUILayout.ObjectField(_levelObjectMap, typeof(LevelObjectMap), true) as LevelObjectMap;

            if (!_terrainMap || !_levelObjectMap)
                GUILayout.Label("Missing terrain and level object data maps.", _errorStyle);
        }

        void DrawTerrainTilemapUI()
        {
            GUILayout.Label("Terrain Tilemap : ", EditorStyles.boldLabel);
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

        void DrawLevelObjectsTilemapUI()
        {
            GUILayout.Label("Level Objects Tilemap : ", EditorStyles.boldLabel);
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
                    item.itemName = (ItemType) EditorGUILayout.EnumPopup(item.itemName);
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
                pending.itemName = (ItemType) EditorGUILayout.EnumPopup(pending.itemName);
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
                item.itemName = (ItemType) EditorGUILayout.EnumPopup(item.itemName);
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
            pending.itemName = (ItemType) EditorGUILayout.EnumPopup(pending.itemName);
            pending.amount = EditorGUILayout.IntField(pending.amount);

            if (EditorGUI.EndChangeCheck()) _pendingAddInventory = pending;

            if (GUILayout.Button(_plus)) ItemAmount.Add(_inventoryOnLoad, _pendingAddInventory);
            GUILayout.EndHorizontal();
        }


        bool ValidFields()
        {
            return _baby && _player && _terrainTileMap && _levelObjectsTileMap && _terrainMap && _levelObjectMap;
        }

        public static void DrawLineUI(Color? color = null, int thickness = 2, int padding = 10)
        {
            var r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            color = color ?? Color.white;
            EditorGUI.DrawRect(r, color.Value);
        }

        List<GameObject> GetObjectsInCell(GridLayout grid, Transform parent, Vector3Int position)
        {
            var results = new List<GameObject>();
            var childCount = parent.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var child = parent.GetChild(i);
                if (position == grid.WorldToCell(child.position)) results.Add(child.gameObject);
            }

            return results;
        }

        public static void InstantiateLevelObjectPrefabInCell(GridLayout grid, GameObject brushTarget,
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
    }
}