using System;
using UnityEngine;

namespace FeedTheBaby.Pathfinding
{
    [Serializable]
    public class Node : IHeapItem<Node>
    {
        public bool traversable;
        public Vector2 worldPosition;
        public Vector2Int gridPosition;

        public int gCost;
        public int hCost;
        public Node parent;

        int heapIndex;

        public Node(bool traversable, Vector2 worldPosition, Vector2Int gridPosition)
        {
            this.traversable = traversable;
            this.worldPosition = worldPosition;
            this.gridPosition = gridPosition;
        }

        public int FCost => gCost + hCost;

        public int CompareTo(Node other)
        {
            int compare = FCost.CompareTo(other.FCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(other.hCost);
            }

            return -compare;
        }

        public int HeapIndex
        {
            get { return heapIndex; }
            set { heapIndex = value; }
        }
    }
}

