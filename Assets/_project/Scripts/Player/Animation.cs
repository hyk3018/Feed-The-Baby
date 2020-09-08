using UnityEngine;

namespace FeedTheBaby.Player
{
    [RequireComponent(typeof(Feeder))]
    public class Animation : MonoBehaviour
    {
        static readonly int Movement = Animator.StringToHash("Movement");
        static readonly int LastDirX = Animator.StringToHash("LastDirX");
        static readonly int LastDirY = Animator.StringToHash("LastDirY");
        static readonly int DirX = Animator.StringToHash("DirX");
        static readonly int DirY = Animator.StringToHash("DirY");

        BehaviourController _behaviour;
        Animator _animator;
        TileMapMovement _tileMapMovement;

        Vector2 _lastMovementBeforeStop = Vector2.down;

        void Awake()
        {
            _behaviour = GetComponent<BehaviourController>();
            _animator = GetComponent<Animator>();
            _tileMapMovement = GetComponent<TileMapMovement>();
        }

        void Update()
        {
            if (LevelManager.Instance.playing)
            {
                _animator.speed = 1;
                Animate();
            }
            else
            {
                _animator.speed = 0;
            }
        }

        void Animate()
        {
            // Only need to update last directions if not moving or interacting since
            // the animator won't need to use it
            Vector2 currentDirectionNormalized = _tileMapMovement.currentDirection.normalized;
            
            if (_tileMapMovement.currentDirection.x == 0 && _tileMapMovement.currentDirection.y == 0
                || !_behaviour.canMove)
            {
                _animator.SetBool(Movement, false);
                _animator.SetFloat(LastDirX, _lastMovementBeforeStop.x);
                _animator.SetFloat(LastDirY, _lastMovementBeforeStop.y);
            }
            else
            {
                _animator.SetBool(Movement, true);
                _lastMovementBeforeStop = currentDirectionNormalized;
            }
            
            _animator.SetFloat(DirX, currentDirectionNormalized.x);
            _animator.SetFloat(DirY, currentDirectionNormalized.y);
        }
    }
}