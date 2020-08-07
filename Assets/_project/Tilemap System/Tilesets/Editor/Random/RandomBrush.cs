using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace FeedTheBaby.Brushes
{
    /// <summary>
    /// This Brush helps to place random Tiles onto a Tilemap.
    /// Use this as an example to create brushes which store specific data per brush and to make brushes which randomize behaviour.
    /// </summary>
    [CustomGridBrush(false, true, false, "Aiyo GG")]
    [CreateAssetMenu(fileName = "New Random Brush", menuName = "Brushes/Random Brush", order = 1)]
    public class RandomBrush : GridBrush
    {
        public struct SizeEnumerator : IEnumerator<Vector3Int>
        {
            readonly Vector3Int _min, _max, _delta;
            Vector3Int _current;

            public SizeEnumerator(Vector3Int min, Vector3Int max, Vector3Int delta)
            {
                _min = _current = min;
                _max = max;
                _delta = delta;
                Reset();
            }

            public SizeEnumerator GetEnumerator()
            {
                return this;
            }

            public bool MoveNext()
            {
                if (_current.z >= _max.z)
                    return false;

                _current.x += _delta.x;
                if (_current.x >= _max.x)
                {
                    _current.x = _min.x;
                    _current.y += _delta.y;
                    if (_current.y >= _max.y)
                    {
                        _current.y = _min.y;
                        _current.z += _delta.z;
                        if (_current.z >= _max.z)
                            return false;
                    }
                }

                return true;
            }

            public void Reset()
            {
                _current = _min;
                _current.x -= _delta.x;
            }

            public Vector3Int Current => _current;

            object IEnumerator.Current => Current;

            void IDisposable.Dispose()
            {
            }
        }

        /// <summary>
        /// A data structure for storing a set of Tiles used for randomization
        /// </summary>
        [Serializable]
        public struct RandomTileSet
        {
            // A set of tiles to be painted as a set
            public TileBase[] randomTiles;
        }

        /// <summary>
        /// The size of a RandomTileSet
        /// </summary>
        public Vector3Int randomTileSetSize = Vector3Int.one;

        /// <summary>
        /// An array of RandomTileSets to choose from when randomizing 
        /// </summary>
        public RandomTileSet[] randomTileSets;

        public int[] randomTileSetProbabilities;
        int _probabilitiesTotal = 0;

        /// <summary>
        /// A flag to determine if picking will add new RandomTileSets 
        /// </summary>
        public bool pickRandomTiles;

        /// <summary>
        /// A flag to determine if picking will add to existing RandomTileSets 
        /// </summary>
        public bool addToRandomTiles;

        /// <summary>
        /// Can be used to increase the brush size, different from size of tile set
        /// </summary>
        public int brushSize;

        /// <summary>
        /// Paints RandomTileSets into a given position within the selected layers.
        /// The RandomBrush overrides this to provide randomized painting functionality.
        /// </summary>
        /// <param name="gridLayout">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to paint data to.</param>
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (randomTileSets != null && randomTileSets.Length > 0)
            {
                if (brushTarget == null)
                    return;

                var tilemap = brushTarget.GetComponent<Tilemap>();
                if (tilemap == null)
                    return;

                var min = position - pivot;
                var bigSize = randomTileSetSize * brushSize;
                foreach (var startLocation in new SizeEnumerator(min, min + bigSize, randomTileSetSize))
                {
                    // var randomTileSet = randomTileSets[(int) (randomTileSets.Length * UnityEngine.Random.value)];
                    var randomTileSet = GetRandomTileBasedOnProbability();
                    var randomBounds = new BoundsInt(startLocation, randomTileSetSize);
                    tilemap.SetTilesBlock(randomBounds, randomTileSet.randomTiles);
                }
            }
            else
            {
                base.Paint(grid, brushTarget, position);
            }
        }

        public RandomTileSet GetRandomTileBasedOnProbability()
        {
            var ticketNumber = Random.Range(1, _probabilitiesTotal + 1);
            for (var i = 0; i < randomTileSets.Length; i++)
            {
                ticketNumber -= randomTileSetProbabilities[i];
                if (ticketNumber <= 0) return randomTileSets[i];
            }

            return default;
        }

        /// <summary>
        /// Picks RandomTileSets given the coordinates of the cells.
        /// The RandomBrush overrides this to provide picking functionality for RandomTileSets.
        /// </summary>
        /// <param name="gridLayout">Grid to pick data from.</param>
        /// <param name="brushTarget">Target of the picking operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cells to paint data from.</param>
        /// <param name="pickStart">Pivot of the picking brush.</param>
        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt bounds, Vector3Int pickStart)
        {
            base.Pick(gridLayout, brushTarget, bounds, pickStart);
            if (!pickRandomTiles)
                return;

            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            var i = 0;
            var count = (bounds.size.x + randomTileSetSize.x - 1) / randomTileSetSize.x
                        * ((bounds.size.y + randomTileSetSize.y - 1) / randomTileSetSize.y)
                        * ((bounds.size.z + randomTileSetSize.z - 1) / randomTileSetSize.z);
            if (addToRandomTiles)
            {
                i = randomTileSets != null ? randomTileSets.Length : 0;
                count += i;
            }

            Array.Resize(ref randomTileSets, count);

            foreach (var startLocation in new SizeEnumerator(bounds.min, bounds.max, randomTileSetSize))
            {
                randomTileSets[i].randomTiles =
                    new TileBase[randomTileSetSize.x * randomTileSetSize.y * randomTileSetSize.z];
                var randomBounds = new BoundsInt(startLocation, randomTileSetSize);
                var j = 0;
                foreach (var pos in randomBounds.allPositionsWithin)
                {
                    var tile = pos.x < bounds.max.x && pos.y < bounds.max.y && pos.z < bounds.max.z
                        ? tilemap.GetTile(pos)
                        : null;
                    randomTileSets[i].randomTiles[j++] = tile;
                }

                i++;
            }
        }

        public void UpdateProbabilityTotal()
        {
            _probabilitiesTotal = randomTileSetProbabilities.Sum();
        }

        public float TileProbability(int i)
        {
            if (_probabilitiesTotal == 0) return 0;
            return (float) randomTileSetProbabilities[i] / _probabilitiesTotal;
        }
    }
}