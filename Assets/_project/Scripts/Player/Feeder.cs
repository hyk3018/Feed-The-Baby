using System;
using System.Collections.Generic;
using System.Linq;
using FeedTheBaby.Commands;
using FeedTheBaby.Game;
using UnityEngine;

namespace FeedTheBaby.Player
{
    public class Feeder : MonoBehaviour, IInteracter
    {
        BehaviourController _behaviour;
        Inventory _inventory;

        bool _feeding;

        void Awake()
        {
            _behaviour = GetComponent<BehaviourController>();
            _inventory = GetComponent<Inventory>();
        }

        void FeedFromInventory(IFeedable feedable)
        {
            if (feedable != null)
            {
                // First we need to find out how much food left do we need to feed baby
                var hunger = feedable.GetHunger();
                feedable.Feed(TakeFoodFromInventory(hunger));
            }
        }

        List<ItemAmount> TakeFoodFromInventory(List<ItemAmount> hunger)
        {
            if (hunger == null || hunger.Count == 0)
                return null;

            // Take items will return at max the hunger required
            // If not fulfilled, still takes from the inventory
            return _inventory.TakeItems(hunger);
        }

        public void Interact(Transform target, Action<bool> interactionEnd)
        {
            if (target == null || !(target.GetComponent<IFeedable>() is IFeedable feedable))
                interactionEnd(false);
            else
            {
                FeedFromInventory(feedable);
                interactionEnd(true);
            }
        }
    }
}