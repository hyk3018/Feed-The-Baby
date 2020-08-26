using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FeedTheBaby.Pathfinding
{
    [RequireComponent(typeof(PathRequestManager))]
    public class Pathfinder : MonoBehaviour
    {
        [SerializeField] NavGrid navGrid = null;
        [SerializeField] GameObject pathNodePrefab = null;
        [SerializeField] bool drawPath = false;

        PathRequestManager _pathRequestManager;
        
        List<GameObject> _pathObjects;

        void Awake()
        {
            _pathRequestManager = GetComponent<PathRequestManager>();
            _pathObjects = new List<GameObject>();
            navGrid = LevelManager.Instance.navigationGrid;
        }
        
        public void StartFindPath(Vector3 startPos, Vector3 targetPos)
        {
            StartCoroutine(FindPath(startPos, targetPos));
        }

        IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
        {
            bool pathSuccess = false;

            if (!navGrid.WithinGrid(startPos) || !navGrid.WithinGrid(targetPos))
            {
                _pathRequestManager.FinishProcessingPath(new List<Vector3>(), false);
                yield break;
            }

            Node startNode = navGrid.NodeFromWorldPoint(startPos);
            Node targetNode = navGrid.NodeFromWorldPoint(targetPos);

            if (startNode == targetNode)
            {
                _pathRequestManager.FinishProcessingPath(new List<Vector3>(), true);
                yield break;
            }

            if (startNode.traversable && targetNode.traversable)
            {

                Heap<Node> openSet = new Heap<Node>(navGrid.MaxSize);
                HashSet<Node> closedSet = new HashSet<Node>();
                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        pathSuccess = true;
                        break;
                    }

                    List<Node> neighbours = navGrid.GetNeighbours(currentNode);
                    foreach (Node neighbour in neighbours)
                    {
                        if (!navGrid.NodePassable(neighbour) && neighbour != targetNode
                            || closedSet.Contains(neighbour))
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
                            else
                                openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
            
            List<Vector3> waypoints = null;
            
            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode.parent);

                if (targetNode.passable)
                    waypoints.Add(targetNode.worldPosition);
                else
                    waypoints.Add(Vector2.Lerp(targetNode.parent.worldPosition, targetNode.worldPosition,
                        0.3f));

                if (drawPath)
                    StartCoroutine(DrawPath(waypoints));
            }

            _pathRequestManager.FinishProcessingPath(waypoints, pathSuccess);
            yield return null;
        }

        List<Vector3> RetracePath(Node start, Node target)
        {
            List<Node> path = new List<Node>();
            Node currentNode = target;
            while (currentNode != start)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Add(start);
            List<Vector3> waypoints = SimplifyPath(path);
            waypoints.Reverse();
            return waypoints;
        }

        List<Vector3> SimplifyPath(List<Node> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i-1].gridPosition.x - path[i].gridPosition.x,
                    path[i-1].gridPosition.y - path[i].gridPosition.y);
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i-1].worldPosition);
                }

                directionOld = directionNew;
            }
            
            if (path.Count > 0)
                waypoints.Add(path[path.Count - 1].worldPosition);
            
            return waypoints;
        }

        IEnumerator DrawPath(List<Vector3> waypoints)
        {
            foreach (GameObject pathObject in _pathObjects)
                Destroy(pathObject);

            foreach (Vector3 pathNode in waypoints)
            {
                GameObject pathObject = Instantiate(pathNodePrefab);
                pathObject.transform.position = pathNode + Vector3.up * 0.5f + Vector3.right * 0.5f;
                _pathObjects.Add(pathObject);
            }

            yield return null;
        }
        
        int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);

            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}