using System;
using UnityEngine;

namespace FeedTheBaby.Player
{
    public class BehaviourController : MonoBehaviour
    {
        public bool canMove;

        void Awake()
        {
            canMove = true;
            LevelManager.Instance.GameEnd += DisableComponents;
        }

        void DisableComponents()
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Movement>().enabled = false;
            GetComponent<Feeder>().enabled = false;
            GetComponent<Harvester>().enabled = false;
            GetComponent<Animator>().enabled = false;
        }
    }
}