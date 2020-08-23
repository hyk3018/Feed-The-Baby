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

        bool _harvesting;

        void Awake()
        {
            _behaviour = GetComponent<BehaviourController>();
            _inventory = GetComponent<Inventory>();
        }

        public void StartHarvest(IHarvestable harvestable)
        {
            if (!_harvesting && harvestable != null)
            {
                // Turn off movement if we need to stay for harvest
                if (harvestable.StayToHarvest()) _behaviour.canMove = false;

                _harvesting = true;
                harvestable.Harvest(CollectHarvest);
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