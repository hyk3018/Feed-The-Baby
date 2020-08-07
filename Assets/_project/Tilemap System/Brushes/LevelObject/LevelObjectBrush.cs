using System.Collections.Generic;
using FeedTheBaby.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Tilemaps;

namespace FeedTheBaby.Brushes
{
    [CustomGridBrush(false, true, false, "Level Object Brush")]
    [CreateAssetMenu(fileName = "Level Object Brush", menuName = "Brushes/Level Object Brush", order = 0)]
    public class LevelObjectBrush : GridBrush
    {
        [SerializeField] LevelObjectType levelObjectType = default;
        [SerializeField] LevelObjectMap levelObjectMap = null;

        GameObject _prefab;
        LevelObjectTile _tile;

        GUIStyle _errorStyle;

        void OnEnable()
        {
            _errorStyle = new GUIStyle
                {normal = {textColor = Color.red}, alignment = TextAnchor.MiddleCenter};
        }

        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (brushTarget.layer == 31 || brushTarget == null) return;

            if (brushTarget.CompareTag("LevelObjects") && levelObjectMap)
                // Fetch the object data
                if (_prefab && _tile || FetchObjectTypeData())
                {
                    // Remove existing tile and game object(s)
                    Erase(grid, brushTarget, position);

                    // Set tile 
                    var tilemap = brushTarget.GetComponent<Tilemap>();
                    if (tilemap) tilemap.SetTile(position, _tile);

                    // Instantiate game object
                    InstantiateLevelObjectPrefabInCell(grid, brushTarget, position);
                }
        }

        public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (brushTarget.layer == 31 || brushTarget.transform == null) return;

            // Remove any game objects
            foreach (var objectInCell in GetObjectsInCell(grid, brushTarget.transform, position))
                Undo.DestroyObjectImmediate(objectInCell);

            // Set tile to null
            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null || !brushTarget.CompareTag("LevelObjects"))
                return;

            tilemap.SetTile(position, null);
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
        }

        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position,
            Vector3Int pickStart)
        {
        }

        public override void FloodFill(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
        }

        bool FetchObjectTypeData()
        {
            _prefab = levelObjectMap.GetPrefab(levelObjectType);
            _tile = levelObjectMap.GetTile(levelObjectType);
            return _prefab && _tile;
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

        void InstantiateLevelObjectPrefabInCell(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            var instance = (GameObject) PrefabUtility.InstantiatePrefab(_prefab);
            if (instance != null)
            {
                Undo.MoveGameObjectToScene(instance, brushTarget.scene, "Paint Prefabs");
                Undo.RegisterCreatedObjectUndo((Object) instance, "Paint Prefabs");
                instance.transform.SetParent(brushTarget.transform);
                instance.transform.position =
                    grid.LocalToWorld(grid.CellToLocalInterpolated(position + new Vector3(0.5f, 0.5f, 0.5f)));
            }
        }
        
        [CustomEditor(typeof(LevelObjectBrush))]
        public class LevelObjectBrushEditor : GridBrushEditor
        {
#pragma warning disable 0649
            SerializedObject _serializedObject = null;
            LevelObjectBrush _levelObjectBrush => target as LevelObjectBrush;
            SerializedProperty levelObjectMap = null;
            SerializedProperty levelObjectType = null;
#pragma warning disable 0649

            protected override void OnEnable()
            {
                base.OnEnable();
                _serializedObject = new SerializedObject(target);
                levelObjectMap = _serializedObject.FindProperty(nameof(levelObjectMap));
                levelObjectType = _serializedObject.FindProperty(nameof(levelObjectType));
            }


            public override void OnPaintInspectorGUI()
            {
                _serializedObject.UpdateIfRequiredOrScript();
                EditorGUILayout.PropertyField(levelObjectMap, true);
                _serializedObject.ApplyModifiedProperties();
                if (!_levelObjectBrush.levelObjectMap)
                    EditorGUILayout.LabelField("Missing level object map.", _levelObjectBrush._errorStyle);

                EditorGUI.BeginChangeCheck();
                _serializedObject.UpdateIfRequiredOrScript();
                EditorGUILayout.PropertyField(levelObjectType, true);
                _serializedObject.ApplyModifiedProperties();
                if (EditorGUI.EndChangeCheck())
                    _levelObjectBrush.FetchObjectTypeData();
            }
        }
    }
}

#endif