using System;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FeedTheBaby.Brushes
{
    /// <summary>
    /// The Brush Editor for a Random Brush.
    /// </summary>
    [CustomEditor(typeof(RandomBrush))]
    public class RandomBrushEditor : GridBrushEditor
    {
        RandomBrush randomBrush => target as RandomBrush;
        GameObject lastBrushTarget;

        /// <summary>
        /// Paints preview data into a cell of a grid given the coordinates of the cell.
        /// The RandomBrush Editor overrides this to draw the preview of the brush for RandomTileSets
        /// </summary>
        /// <param name="gridLayout">Grid to paint data to.</param>
        /// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to paint data to.</param>
        public override void PaintPreview(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (randomBrush.randomTileSets != null && randomBrush.randomTileSets.Length > 0)
            {
                base.PaintPreview(grid, null, position);
                if (brushTarget == null)
                    return;

                var tilemap = brushTarget.GetComponent<Tilemap>();
                if (tilemap == null)
                    return;

                var min = position - randomBrush.pivot;
                foreach (var startLocation in new RandomBrush.SizeEnumerator(min,
                    min + randomBrush.size * randomBrush.brushSize, randomBrush.randomTileSetSize))
                {
                    var randomTileSet = randomBrush.GetRandomTileBasedOnProbability();
                    var randomBounds = new BoundsInt(startLocation, randomBrush.randomTileSetSize);
                    var j = 0;
                    foreach (var pos in randomBounds.allPositionsWithin)
                        tilemap.SetEditorPreviewTile(pos, randomTileSet.randomTiles[j++]);
                }

                lastBrushTarget = brushTarget;
            }
            else
            {
                base.PaintPreview(grid, brushTarget, position);
            }
        }

        /// <summary>
        /// Clears all RandomTileSet previews.
        /// </summary>
        public override void ClearPreview()
        {
            if (lastBrushTarget != null)
            {
                var tilemap = lastBrushTarget.GetComponent<Tilemap>();
                if (tilemap == null)
                    return;

                tilemap.ClearAllEditorPreviewTiles();

                lastBrushTarget = null;
            }
            else
            {
                base.ClearPreview();
            }
        }

        /// <summary>
        /// Callback for painting the GUI for the GridBrush in the Scene View.
        /// The CoordinateBrush Editor overrides this to draw the current coordinates of the brush.
        /// </summary>
        /// <param name="gridLayout">Grid that the brush is being used on.</param>
        /// <param name="brushTarget">Target of the GridBrushBase::ref::Tool operation. By default the currently selected GameObject.</param>
        /// <param name="position">Current selected location of the brush.</param>
        /// <param name="tool">Current GridBrushBase::ref::Tool selected.</param>
        /// <param name="executing">Whether brush is being used.</param>
        public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position,
            GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);

            var labelText = "Pos: " + position.position;
            if (position.size.x > 1 || position.size.y > 1) labelText += " Size: " + position.size;

            Handles.Label(grid.CellToWorld(position.position), labelText);
        }

        /// <summary>
        /// Callback for painting the inspector GUI for the RandomBrush in the Tile Palette.
        /// The RandomBrush Editor overrides this to have a custom inspector for this Brush.
        /// </summary>
        public override void OnPaintInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            randomBrush.brushSize = EditorGUILayout.IntField("Brush Size Multiplier", randomBrush.brushSize);
            randomBrush.pickRandomTiles = EditorGUILayout.Toggle("Pick Random Tiles", randomBrush.pickRandomTiles);
            using (new EditorGUI.DisabledScope(!randomBrush.pickRandomTiles))
            {
                randomBrush.addToRandomTiles =
                    EditorGUILayout.Toggle("Add To Random Tiles", randomBrush.addToRandomTiles);
            }

            EditorGUI.BeginChangeCheck();
            randomBrush.randomTileSetSize =
                EditorGUILayout.Vector3IntField("Tile Set Size", randomBrush.randomTileSetSize);
            if (EditorGUI.EndChangeCheck())
                for (var i = 0; i < randomBrush.randomTileSets.Length; ++i)
                {
                    var sizeCount = randomBrush.randomTileSetSize.x * randomBrush.randomTileSetSize.y *
                                    randomBrush.randomTileSetSize.z;
                    randomBrush.randomTileSets[i].randomTiles = new TileBase[sizeCount];
                }

            var randomTileSetCount = EditorGUILayout.DelayedIntField("Number of Tiles",
                randomBrush.randomTileSets != null ? randomBrush.randomTileSets.Length : 0);

            if (randomTileSetCount < 0)
                randomTileSetCount = 0;
            if (randomBrush.randomTileSets == null || randomBrush.randomTileSets.Length != randomTileSetCount)
            {
                Array.Resize(ref randomBrush.randomTileSets, randomTileSetCount);
                for (var i = 0; i < randomBrush.randomTileSets.Length; ++i)
                {
                    var sizeCount = randomBrush.randomTileSetSize.x * randomBrush.randomTileSetSize.y *
                                    randomBrush.randomTileSetSize.z;
                    if (randomBrush.randomTileSets[i].randomTiles == null
                        || randomBrush.randomTileSets[i].randomTiles.Length != sizeCount)
                        randomBrush.randomTileSets[i].randomTiles = new TileBase[sizeCount];
                }
            }

            if (randomBrush.randomTileSetProbabilities == null ||
                randomBrush.randomTileSetProbabilities.Length != randomTileSetCount)
            {
                var oldLength = randomBrush.randomTileSetProbabilities.Length;
                Array.Resize(ref randomBrush.randomTileSetProbabilities, randomTileSetCount);
                for (var i = oldLength; i < randomBrush.randomTileSetProbabilities.Length; i++)
                    randomBrush.randomTileSetProbabilities[i] = 1;
                randomBrush.UpdateProbabilityTotal();
            }

            if (randomTileSetCount > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Place random tiles.");

                for (var i = 0; i < randomTileSetCount; i++)
                {
                    EditorGUILayout.LabelField("Tile Set " + (i + 1));
                    EditorGUI.BeginChangeCheck();
                    randomBrush.randomTileSetProbabilities[i] =
                        EditorGUILayout.IntField("Probability Allocation ", randomBrush.randomTileSetProbabilities[i]);

                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.FloatField("Tile Set Probability ",
                            randomBrush.TileProbability(i));
                    }

                    if (EditorGUI.EndChangeCheck()) randomBrush.UpdateProbabilityTotal();
                    for (var j = 0; j < randomBrush.randomTileSets[i].randomTiles.Length; ++j)
                        randomBrush.randomTileSets[i].randomTiles[j] = (TileBase) EditorGUILayout.ObjectField(
                            "Tile " + (j + 1), randomBrush.randomTileSets[i].randomTiles[j], typeof(TileBase), false,
                            null);
                }
            }

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(randomBrush);
        }
    }
}