using System;
using UnityEngine;

namespace FeedTheBaby.Player
{
    [RequireComponent(typeof(Inventory))]
    [RequireComponent(typeof(BehaviourController))]
    public class Harvester : MonoBehaviour
    {
        [SerializeField] AudioClip harvestSound = null;
        
        BehaviourController _behaviour;
        Inventory _inventory;
        TileMapMovement _tileMapMovement;

        bool _harvesting;

        void Awake()
        {
            _behaviour = GetComponent<BehaviourController>();
            _tileMapMovement = GetComponent<TileMapMovement>();
            _inventory = GetComponent<Inventory>();
        }

        void OnTriggerStay2D(Collider2D other)
        {
            // If moved into a harvestable, harvest it
            if (_tileMapMovement.MovedInto(other.transform))
            {
                var harvestable = other.GetComponent<IHarvestable>();
                if (harvestable != null && !_harvesting)
                {
                    // Turn off movement if we need to stay for harvest
                    if (harvestable.StayToHarvest()) _behaviour.canMove = false;

                    _harvesting = true;
                    harvestable.StartHarvest(CollectHarvest);
                }
            }
        }

        void CollectHarvest(ItemAmount itemAmount)
        {
            if (_inventory)
            {
                if (_harvesting)
                {
                    _harvesting = false;
                    _behaviour.canMove = true;
                }

                AudioSource.PlayClipAtPoint(harvestSound, transform.position);
                _inventory.AddItem(itemAmount);
            }
        }
    }
}