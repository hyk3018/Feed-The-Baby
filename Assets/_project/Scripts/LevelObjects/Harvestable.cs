using System;
using FeedTheBaby.Commands;
using FeedTheBaby.Player;
using UnityEngine;

namespace FeedTheBaby.LevelObjects
{
    public interface IHarvestable : IInteractable
    {
        void Harvest(Action<HarvestResult> onFinishHarvest);
        bool StayToHarvest();
    }

    public interface IInteractable
    {
        CommandType PossibleCommands();
    }

    [RequireComponent(typeof(Timer))]
    public class Harvestable : LevelObject, IHarvestable
    {
        [SerializeField] ItemAmount itemAmount = default;
        [SerializeField] float harvestTime = 2f;
        [SerializeField] bool stayToHarvest = true;
        [SerializeField] bool destroyOnHarvest = true;

        [SerializeField] bool needsFight;

        Timer _timer;
        Harvester _currentHarvester;
        Grow _grow;
        
        public Action<HarvestResult> OnFinishHarvest;

        void Awake()
        {
            _grow = GetComponent<Grow>();
            _timer = GetComponent<Timer>();
        }

        // Starts the harvest of harvestable and returns whether
        // harvesting should disable the harvester
        public bool StayToHarvest()
        {
            return stayToHarvest;
        }

        public void Harvest(Action<HarvestResult> onFinishHarvest)
        {
            OnFinishHarvest += onFinishHarvest;
            
            if (_grow && !_grow.FullyGrown)
            {
                OnFinishHarvest(new HarvestResult(default, false));
                return;
            }

            if (destroyOnHarvest)
                DestroyTile?.Invoke();
            
            _timer.StartCount(harvestTime);
            _timer.TimerEnd += FinishHarvest;
        }

        void FinishHarvest(Timer t)
        {
            OnFinishHarvest(new HarvestResult(itemAmount,true));
            if (destroyOnHarvest)
                Destroy(gameObject);
        }

        public CommandType PossibleCommands() => CommandType.HARVEST;
    }
    
    public struct HarvestResult
    {
        public bool success;
        public ItemAmount harvest;

        public HarvestResult(ItemAmount harvest, bool success)
        {
            this.harvest = harvest;
            this.success = success;
        }
    }
}