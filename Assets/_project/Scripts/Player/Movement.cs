using System;
using UnityEngine;

namespace FeedTheBaby.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 0f;

        public Vector2 CurrentMovement;

        Rigidbody2D _rb2d;

        BehaviourController _behaviour;

        void Awake()
        {
            _behaviour = GetComponent<BehaviourController>();
            _rb2d = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            Move();
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

        void Move()
        {
            var moveX = Input.GetAxisRaw("Horizontal");
            CurrentMovement.x = moveX;
            var moveY = Input.GetAxisRaw("Vertical");
            CurrentMovement.y = moveY;
            CurrentMovement = CurrentMovement.normalized * (moveSpeed * Time.deltaTime);

            if (_behaviour.canMove)
                _rb2d.velocity = new Vector2(moveX, moveY).normalized * moveSpeed;
            else
                _rb2d.velocity = Vector2.zero;
        }
    }
}