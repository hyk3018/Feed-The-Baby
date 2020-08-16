using System;
using System.Collections;
using FeedTheBaby.Pathfinding;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FeedTheBaby.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class TileMapMovement : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 0f;

        public Vector2 CurrentMovement;

        Rigidbody2D _rb2d;
        BehaviourController _behaviour;

        Vector3 _currentTarget;
        Vector3[] _currentPath;
        int _targetIndex;
        Coroutine _pathFollow;

        void Awake()
        {
            _behaviour = GetComponent<BehaviourController>();
            _rb2d = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _currentTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                PathRequestManager.RequestPath(transform.position, _currentTarget, OnPathFound);
            }
        }

        void OnPathFound(Vector3[] newPath, bool pathSuccessful)
        {
            if (pathSuccessful && newPath.Length > 0)
            {
                _currentPath = newPath;
                _targetIndex = 0;
                if (_pathFollow != null)
                    StopCoroutine(_pathFollow);
                _pathFollow = StartCoroutine(FollowPath());
            }
        }

        IEnumerator FollowPath()
        {
            Vector3 currentWaypoint = _currentPath[0] + new Vector3(0.5f,0.5f);
            while (true)
            {
                if (Vector3.Distance(transform.position, currentWaypoint) < 0.2f)
                {
                    transform.position = currentWaypoint;
                    _targetIndex++;
                    if (_targetIndex >= _currentPath.Length)
                        break;
                    currentWaypoint = _currentPath[_targetIndex] + new Vector3(0.5f, 0.5f);
                }

                Vector2 moveDirection = currentWaypoint - transform.position;
                Move(moveDirection.normalized);
                yield return null;
            }
            
            Move(Vector2.zero);
        }

        // Determines whether we are moving towards another object
        // Useful for when multiple collisions occur and we move away
        // after the first
        public bool MovedInto(Transform otherTransform)
        {
            var toTransform = transform.position - otherTransform.position;
            var target = new Vector2(toTransform.x, toTransform.y);
            return Vector2.Dot(CurrentMovement, target) < 0;
        }

        void Move(Vector2 normalizedDirection)
        {
            CurrentMovement = normalizedDirection * (moveSpeed * Time.deltaTime);

            if (_behaviour.canMove)
                // _rb2d.velocity = new Vector2(normalizedDirection.x, normalizedDirection.y).normalized * moveSpeed;
                _rb2d.velocity = normalizedDirection * moveSpeed;
            else
                _rb2d.velocity = Vector2.zero;
        }
    }
}