using System;
using System.Collections.Generic;
using FeedTheBaby.Tilemaps.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FeedTheBaby.Pathfinding
{
    [Serializable]
    public class NavGrid
    {
        [SerializeField] Node[] grid;
        [SerializeField] BoundsInt gridBounds;

        public Tilemap terrainTileMap;

        public int MaxSize => gridBounds.size.x * gridBounds.size.y;

        public NavGrid(Tilemap terrainTileMap, Tilemap obstructionsTileMap, Tilemap levelObjectsTileMap)
        {
            this.terrainTileMap = terrainTileMap;
            terrainTileMap.CompressBounds();
            gridBounds = terrainTileMap.cellBounds;
            grid = new Node[gridBounds.size.y * gridBounds.size.x];
            FillGrid(terrainTileMap, obstructionsTileMap, levelObjectsTileMap);
        }
        
        // For each position in the grid we decide whether it is traversable or not by performing
        // tile map lookups
        
        void FillGrid(Tilemap terrainTileMap, Tilemap obstructionsTileMap, Tilemap levelObjectsTileMap)
        {
            foreach (Vector3Int cellPosition in gridBounds.allPositionsWithin)
            {
                Vector3 worldPosition = terrainTileMap.CellToWorld(cellPosition);

                bool traversable = terrainTileMap.HasTile(cellPosition) && !obstructionsTileMap.HasTile(cellPosition);
                bool passable = traversable;
                if (traversable)
                {
                    bool hasLevelObjectTile = levelObjectsTileMap.HasTile(cellPosition);
                    if (hasLevelObjectTile)
                        if (levelObjectsTileMap.GetTile(cellPosition) is LevelObjectTile levelObjectTile)
                            passable = levelObjectTile.passable;
                }

                traversable = !obstructionsTileMap.HasTile(cellPosition) && traversable;

                // All positions have to be relative to the min (bottom left) rather than center 
                int yRelativeToBottomLeft = cellPosition.y - gridBounds.yMin;
                int xRelativeToBottomLeft = cellPosition.x - gridBounds.xMin;
                
                Node node = new Node(traversable, passable, worldPosition, new Vector2Int(xRelativeToBottomLeft,
                    yRelativeToBottomLeft));
                
                grid[GridCoordToIndex(yRelativeToBottomLeft, xRelativeToBottomLeft)] = node;
            }
        }

        int GridCoordToIndex(int y, int x)
        {
            return y * gridBounds.size.x + x;
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            int xMin = (node.gridPosition.x == 0) ? 0 : -1;
            int xMax = (node.gridPosition.x == gridBounds.size.x - 1) ? 0 : 1;
            int yMin = (node.gridPosition.y == 0) ? 0 : -1;
            int yMax = (node.gridPosition.y == gridBounds.size.y - 1) ? 0 : 1;
            
            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    if (Mathf.Abs(x) + Mathf.Abs(y) == 2)
                        continue;
                    
                    neighbours.Add(grid[(y + node.gridPosition.y) * gridBounds.size.x +
                                        (x + node.gridPosition.x) ]);
                }
            }

            return neighbours;
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            Vector3Int cellPosition = terrainTileMap.WorldToCell(worldPosition);
            int xRelativeToBottomleft = cellPosition.x - gridBounds.xMin;
            int yRelativeToBottomLeft = cellPosition.y - gridBounds.yMin;
            return grid[yRelativeToBottomLeft * gridBounds.size.x + xRelativeToBottomleft];
        }

        public bool NodePassable(Node neighbour)
        {
            return neighbour.passable;
        }
    }
}