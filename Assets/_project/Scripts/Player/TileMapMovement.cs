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

        public Vector2 CurrentMovement;
        public bool lockMovement;

        Rigidbody2D _rb2d;
        BehaviourController _behaviour;
        Vector3 _currentTarget;
        List<Vector3> _currentPath;
        int _targetIndex;
        bool _reachedTarget;
        bool _alreadyFailed;
        
        Coroutine _pathFollow;
        Coroutine _moveToTransform;

        Action<bool> _onCommandFinish;

        void Awake()
        {
            _behaviour = GetComponent<BehaviourController>();
            _rb2d = GetComponent<Rigidbody2D>();
        }

        IEnumerator FollowPath()
        {
            Vector3 currentWaypoint = _currentPath[0] + new Vector3(0.5f, 0.5f);
            while (!lockMovement)
            {
                if (Vector3.Distance(transform.position, currentWaypoint) < 0.2f)
                {
                    transform.position = currentWaypoint;
                    _targetIndex++;
                    if (_targetIndex >= _currentPath.Count)
                    {
                        break;
                    }
                    currentWaypoint = _currentPath[_targetIndex] + new Vector3(0.5f, 0.5f);
                }

                Vector2 moveDirection = currentWaypoint - transform.position;
                Move(moveDirection.normalized);
                yield return null;
            }

            FinishMove();
        }

        void FinishMove()
        {
            Move(Vector2.zero);
            _reachedTarget = true;
            _onCommandFinish(true);
        }

        public void ExecuteMoveTransform(MoveTransformCommand moveCommand, Action<bool> onCommandFinish)
        {
            _onCommandFinish = onCommandFinish;
            _reachedTarget = false;
            _alreadyFailed = false;
            _moveToTransform = StartCoroutine(MoveToTransform(moveCommand.target));
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

        public void ExecuteMovePosition(MovePositionCommand moveCommand, Action<bool> onCommandFinish)
        {
            _currentTarget = moveCommand.target;
            _alreadyFailed = false;
            _onCommandFinish = onCommandFinish;
            MoveToPosition();
        }

        void MoveToPosition()
        { 
            PathRequestManager.RequestPath(transform.position, _currentTarget, OnPathFound);
        }

        void OnPathFound(List<Vector3> newPath, bool pathSuccessful)
        {
            if (_alreadyFailed)
                return;

            if (_pathFollow != null)
                StopCoroutine(_pathFollow);

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
                    FinishMove();
                }
            }
            else
            {
                if (_moveToTransform != null)
                {
                    StopCoroutine(_moveToTransform);
                }
                
                _alreadyFailed = true;
                _onCommandFinish(false);
            }
        }

        public void Interrupt()
        {
            _onCommandFinish?.Invoke(false);
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
                _rb2d.velocity = normalizedDirection * moveSpeed;
            else
                _rb2d.velocity = Vector2.zero;
        }
    }
}