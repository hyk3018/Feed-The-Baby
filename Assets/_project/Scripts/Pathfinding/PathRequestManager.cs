using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FeedTheBaby.Pathfinding
{
    public class PathRequestManager : MonoBehaviour
    {
        Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
        PathRequest currentPathRequest;

        static PathRequestManager _instance;
        Pathfinder _pathfinder;

        bool isProcessingPath;

        void Awake()
        {
            _pathfinder = GetComponent<Pathfinder>();
            _instance = this;
        }

        public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<List<Vector3>, bool> callback)
        {
            PathRequest request = new PathRequest(pathStart, pathEnd, callback);
            _instance.pathRequestQueue.Enqueue(request);
            _instance.TryProcessNext();
        }

        void TryProcessNext()
        {
            if (!isProcessingPath && pathRequestQueue.Count > 0)
            {
                currentPathRequest = pathRequestQueue.Dequeue();
                isProcessingPath = true;
                _pathfinder.StartFindPath(currentPathRequest.pathStart,
                    currentPathRequest.pathEnd);
            }
        }

        public void FinishProcessingPath(List<Vector3> path, bool success)
        {
            currentPathRequest.callback(path, success);
            isProcessingPath = false;
            TryProcessNext();
        }
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<List<Vector3>, bool> callback;

        public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<List<Vector3>, bool> callback)
        {
            this.pathStart = pathStart;
            this.pathEnd = pathEnd;
            this.callback = callback;
        }
    }
}