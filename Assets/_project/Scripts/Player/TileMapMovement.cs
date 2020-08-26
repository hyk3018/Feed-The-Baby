using System;
using System.Collections;
using System.Collections.Generic;
using FeedTheBaby.Commands;
using FeedTheBaby.Pathfinding;
using UnityEngine;

namespace FeedTheBaby.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class TileMapMovement : MonoBehaviour, IMoveCommandExecutor
    {
        [SerializeField] float moveSpeed = 0f;
        [SerializeField] float recalculatePathRate = 0f;

        public Vector2 currentDirection;
        public bool lockMovement;

        Rigidbody2D _rb2d;
        BehaviourController _behaviour;
        
        Vector3 _currentTarget;
        Vector3 _currentWaypoint;
        List<Vector3> _currentPath;
        int _targetIndex;
        bool _reachedTarget;
        bool _alreadyFailed;

        Coroutine _pathFollow;
        Coroutine _moveToTransform;

        Action<bool> _onCommandFinish;

        void Awake()
        {
            _reachedTarget = true;
            _behaviour = GetComponent<BehaviourController>();
            _rb2d = GetComponent<Rigidbody2D>();
        }
        
        #region Post Path Found and Movement

        void OnPathFound(List<Vector3> newPath, bool pathSuccessful)
        {
            if (_alreadyFailed)
                return;

            if (pathSuccessful)
            {
                _currentPath = newPath;
                _targetIndex = 0;
                
                if (newPath.Count > 0)
                {
                    _pathFollow = StartCoroutine(FollowPath());
                }
                else
                {
                    FinishMove(true);
                }
            }
            else
            {
                FinishMove(false);
            }
        }

        IEnumerator FollowPath()
        {
            _currentWaypoint = _currentPath[0] + new Vector3(0.5f, 0.5f);

            if (_currentPath.Count > 1)
            {
                if (BetweenFirstTwoWaypoints())
                {
                    _currentWaypoint = _currentPath[1] + new Vector3(0.5f, 0.5f);
                    _targetIndex++;
                }
            }

            while (!lockMovement)
            {
                if (Vector2.Distance(transform.position, _currentWaypoint) < 0.1f)
                {
                    transform.position = _currentWaypoint;
                    _targetIndex++;
                    if (_targetIndex >= _currentPath.Count)
                    {
                        break;
                    }
                    _currentWaypoint = _currentPath[_targetIndex] + new Vector3(0.5f, 0.5f);
                }

                Vector2 moveDirection = _currentWaypoint - transform.position;
                Move(moveDirection.normalized);
                yield return null;
            }

            FinishMove(true);
        }
        
        void Move(Vector2 normalizedDirection)
        {
            Vector2 currentVelocity = normalizedDirection * moveSpeed;
            currentDirection = currentVelocity;

            if (_behaviour.canMove)
                _rb2d.velocity = currentVelocity;
            else
                _rb2d.velocity = Vector2.zero;
        }

        // Checks if we are currently between first and second waypoint,
        // and goes to second waypoint if true
        bool BetweenFirstTwoWaypoints()
        {
            Vector3 firstWaypoint = _currentPath[0] + new Vector3(0.5f, 0.5f);
            Vector3 secondWaypoint = _currentPath[1] + new Vector3(0.5f, 0.5f);
            var currentPosition = transform.position;
            
            float a = Vector3.Dot(Vector3.Cross(firstWaypoint, currentPosition),
                Vector3.Cross(firstWaypoint, secondWaypoint));
            float b = Vector3.Dot(Vector3.Cross(secondWaypoint, currentPosition),
                Vector3.Cross(secondWaypoint, firstWaypoint));

            return (a >= 0 && b >= 0);
        }
        
        #endregion
        
        #region Logic For Different Movement States
        
        public void ExecuteMoveTransform(MoveTransformCommand moveCommand, Action<bool> onCommandFinish)
        {
            _reachedTarget = false;
            _alreadyFailed = false;
            _onCommandFinish = onCommandFinish;
            _moveToTransform = StartCoroutine(MoveToTransform(moveCommand.target));
        }

        public void ExecuteMovePosition(MovePositionCommand moveCommand, Action<bool> onCommandFinish)
        {
            _currentTarget = moveCommand.target;
            _reachedTarget = false;
            _alreadyFailed = false;
            _onCommandFinish = onCommandFinish;
            MoveToPosition();
        }

        IEnumerator MoveToTransform(Transform target)
        {
            while (!_reachedTarget)
            {
                _currentTarget = target.position;
                MoveToPosition();
                yield return new WaitForSeconds(recalculatePathRate);
            }
        }

        void MoveToPosition()
        { 
            PathRequestManager.RequestPath(transform.position, _currentTarget, OnPathFound);
        }

        public void Interrupt()
        {
            if (_pathFollow == null && _moveToTransform == null)
                return;
            
            FinishMove(false);
        }
        
        void FinishMove(bool success)
        {
            if (_moveToTransform != null)
            {
                StopCoroutine(_moveToTransform);
                _moveToTransform = null;
            }

            if (_pathFollow != null)
            {
                StopCoroutine(_pathFollow);
                _pathFollow = null;
            }

            if (!success)
                _alreadyFailed = true;

            Move(Vector2.zero);
            _reachedTarget = success;
            
            _onCommandFinish(success);
        }
        
        #endregion

        // Determines whether we are moving towards another object
        // Useful for when multiple collisions occur and we move away
        // after the first
        public bool MovedInto(Transform otherTransform)
        {
            var toTransform = transform.position - otherTransform.position;
            var target = new Vector2(toTransform.x, toTransform.y);
            return Vector2.Dot(currentDirection, target) < 0;
        }
    }
}