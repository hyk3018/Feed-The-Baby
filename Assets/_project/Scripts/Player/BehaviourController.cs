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
            LevelManager.Instance.LevelEnd += DisableComponents;
        }

        void DisableComponents()
        {
            canMove = false;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            TileMapMovement tileMapMovement = GetComponent<TileMapMovement>();
            tileMapMovement.lockMovement = true;
            tileMapMovement.enabled = false;
            GetComponent<Feeder>().enabled = false;
            GetComponent<Harvester>().enabled = false;
            GetComponent<Animator>().enabled = false;
        }
    }
}