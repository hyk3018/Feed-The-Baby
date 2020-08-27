using System;
using FeedTheBaby.Commands;
using FeedTheBaby.Player;
using UnityEngine;

namespace FeedTheBaby.LevelObjects
{
    public interface IHarvestable
    {
        void Harvest(Action<ItemAmount> onFinishHarvest);
        bool StayToHarvest();
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
        public Action<ItemAmount> OnFinishHarvest;

        void Awake()
        {
            _timer = GetComponent<Timer>();
        }

        // Starts the harvest of harvestable and returns whether
        // harvesting should disable the harvester
        public bool StayToHarvest()
        {
            return stayToHarvest;
        }

        public void Harvest(Action<ItemAmount> onFinishHarvest)
        {
            if (destroyOnHarvest)
                DestroyTile();
            OnFinishHarvest += onFinishHarvest;
            _timer.StartCount(harvestTime);
            _timer.TimerEnd += FinishHarvest;
        }

        void FinishHarvest(Timer t)
        {
            OnFinishHarvest(itemAmount);
            if (destroyOnHarvest)
                Destroy(gameObject);
        }
    }
}