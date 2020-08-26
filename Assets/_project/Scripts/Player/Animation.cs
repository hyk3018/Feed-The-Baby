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

        Vector2 _lastMovement = Vector2.down;

        void Awake()
        {
            _behaviour = GetComponent<BehaviourController>();
            _animator = GetComponent<Animator>();
            _tileMapMovement = GetComponent<TileMapMovement>();
        }

        void Update()
        {
            Animate();
        }

        void Animate()
        {
            // Only need to update last directions if not moving or interacting since
            // the animator won't need to use it
            if (_tileMapMovement.currentDirection.x == 0 && _tileMapMovement.currentDirection.y == 0
                || !_behaviour.canMove)
            {
                _animator.SetBool(Movement, false);
                _animator.SetFloat(LastDirX, _lastMovement.x);
                _animator.SetFloat(LastDirY, _lastMovement.y);
            }
            else
            {
                _animator.SetBool(Movement, true);
                _lastMovement = _tileMapMovement.currentDirection.normalized;
            }

            _animator.SetFloat(DirX, _tileMapMovement.currentDirection.normalized.x);
            _animator.SetFloat(DirY, _tileMapMovement.currentDirection.normalized.y);
        }
    }
}