using System;
using FeedTheBaby.Commands;
using FeedTheBaby.LevelObjects;
using UnityEngine;

namespace FeedTheBaby.Player
{
    [RequireComponent(typeof(Inventory))]
    [RequireComponent(typeof(BehaviourController))]
    public class Harvester : MonoBehaviour, IInteracter
    {
        [SerializeField] AudioClip harvestSound = null;
        
        BehaviourController _behaviour;
        Inventory _inventory;

        bool _harvesting;

        Action<bool> _onHarvestEnd;

        void Awake()
        {
            _behaviour = GetComponent<BehaviourController>();
            _inventory = GetComponent<Inventory>();
        }

        void StartHarvest(IHarvestable harvestable)
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
                _onHarvestEnd(true);
            }

            _onHarvestEnd(false);
        }

        public void Interact(Transform target, Action<bool> interactionEnd)
        {
            if (target == null || !(target.GetComponent<IHarvestable>() is IHarvestable harvestable)) 
                interactionEnd(false);
            else
            {
                _onHarvestEnd += interactionEnd;
                StartHarvest(harvestable);
            }
        }
    }
}