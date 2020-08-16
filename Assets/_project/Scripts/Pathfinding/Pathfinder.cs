using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FeedTheBaby.Pathfinding
{
    public class Pathfinder : MonoBehaviour
    {
        [SerializeField] NavGrid navGrid = null;
        [SerializeField] Transform seeker = null;
        [SerializeField] Transform target = null;
        [SerializeField] Tilemap terrainTileMap = null;
        [SerializeField] GameObject pathNodePrefab = null;

        List<GameObject> _pathObjects;
        
        List<Node> _path = new List<Node>();

        void Awake()
        {
            _pathObjects = new List<GameObject>();
            navGrid = LevelManager.Instance.currentLevelData.navigationGrid;
            navGrid.terrainTileMap = terrainTileMap;
            StartCoroutine(PeriodicPathFind());
        }

        IEnumerator PeriodicPathFind()
        {
            while (true)
            {
                FindPath(seeker.position, target.position);
                DrawPath();
                yield return new WaitForSeconds(.1f);
            }
        }

        void DrawPath()
        {
            foreach (GameObject pathObject in _pathObjects)
                Destroy(pathObject);

            foreach (Node pathNode in _path)
            {
                GameObject pathObject = Instantiate(pathNodePrefab);
                pathObject.transform.position = pathNode.worldPosition + Vector2.up * 0.5f + Vector2.right * 0.5f;
                _pathObjects.Add(pathObject);
            }
        }

        void FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Node startNode = navGrid.NodeFromWorldPoint(startPos);
            Node targetNode = navGrid.NodeFromWorldPoint(targetPos);
            
            Heap<Node> openSet = new Heap<Node>(navGrid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    RetracePath(startNode, targetNode);
                    return;
                }

                List<Node> neighbours = navGrid.GetNeighbours(currentNode);
                foreach (Node neighbour in neighbours)
                {
                    if (!navGrid.NodeTraversable(neighbour) || closedSet.Contains(neighbour))
                        continue;

                    int newMoveCostToNeighbour = currentNode.gCost +
                                              GetDistance(currentNode, neighbour);
                    if (newMoveCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMoveCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;
                        
                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }

        void RetracePath(Node start, Node target)
        {
            _path = new List<Node>();
            Node currentNode = target;
            while (currentNode != start)
            {
                _path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            
            _path.Reverse();
        }
        
        int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);

            return 14 * dstX + 10 * (dstY - dstX);
        }

        // void OnDrawGizmos()
        // {
        //     foreach (Node node in _path)
        //     {
        //         Gizmos.color = Color.blue;
        //         Gizmos.DrawSphere(node.worldPosition, 1);
        //     }
        // }
    }
}