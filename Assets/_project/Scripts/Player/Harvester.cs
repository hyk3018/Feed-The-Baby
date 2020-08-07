using System;
using UnityEngine;

namespace FeedTheBaby.Player
{
    [RequireComponent(typeof(Inventory))]
    [RequireComponent(typeof(BehaviourController))]
    public class Harvester : MonoBehaviour
    {
        BehaviourController _behaviour;
        Inventory _inventory;
        Movement _movement;

        bool _harvesting;

        void Awake()
        {
            _behaviour = GetComponent<BehaviourController>();
            _movement = GetComponent<Movement>();
            _inventory = GetComponent<Inventory>();
        }

        void OnTriggerStay2D(Collider2D other)
        {
            // If moved into a harvestable, harvest it
            if (_movement.MovedInto(other.transform))
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

                _inventory.AddItem(itemAmount);
            }
        }
    }
}