using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FeedTheBaby.Player
{
    public class Feeder : MonoBehaviour
    {
        BehaviourController _behaviour;
        Inventory _inventory;

        bool _feeding;
        IFeedable _feedable;

        void Awake()
        {
            _behaviour = GetComponent<BehaviourController>();
            _inventory = GetComponent<Inventory>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var feedable = other.GetComponent<IFeedable>();
            if (feedable != null) _feedable = feedable;
        }

        void OnTriggerExit2D(Collider2D other)
        {
            var feedable = other.GetComponent<IFeedable>();
            if (feedable == _feedable) _feedable = null;
        }

        void Update()
        {
            // If in contact with feedable and space pressed then feed it
            if (Input.GetKeyDown(KeyCode.Space) && _feedable != null)
            {
                // First we need to find out how much food left do we need to feed baby
                var hunger = _feedable.GetHunger();
                _feedable.Feed(TakeFoodFromInventory(hunger));
            }
        }

        public List<ItemAmount> TakeFoodFromInventory(List<ItemAmount> hunger)
        {
            if (hunger == null || hunger.Count == 0)
                return null;

            // Take items will return at max the hunger required
            // If not fulfilled, still takes from the inventory
            return _inventory.TakeItems(hunger);
        }
    }
}