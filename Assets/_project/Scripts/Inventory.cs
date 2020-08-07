﻿using System;
using System.Collections.Generic;
using System.Linq;
using FeedTheBaby.GameData;
using FeedTheBaby.UI;
using UnityEngine;
using UnityEngine.UI;

namespace FeedTheBaby
{
    //
    // [CreateAssetMenu(fileName = "Inventory", menuName = "FeedTheBaby/Inventory", order = 20)]
    public class Inventory : MonoBehaviour
    {
        [SerializeField] List<ItemAmount> itemList = null;

        public Action<List<ItemAmount>> InventoryChange;

        void Awake()
        {
            itemList = LevelManager.Instance.currentLevelData.initialInventory.ToList();
        }

        void Start()
        {
            InventoryChange(itemList);
        }

        public void AddItem(ItemAmount itemAmount)
        {
            var added = false;

            for (var i = 0; i < itemList.Count; i++)
                if (itemList[i].itemName == itemAmount.itemName)
                {
                    added = true;
                    itemList[i] = new ItemAmount(itemAmount.itemName,
                        itemList[i].amount + itemAmount.amount);
                    break;
                }

            if (!added)
                itemList.Add(itemAmount);

            InventoryChange(itemList);
        }

        // Take from inventory, if there is not enough of a particular item, take it anyway
        public List<ItemAmount> TakeItems(List<ItemAmount> itemsToTake)
        {
            var toTake = new List<ItemAmount>();
            var toRemain = new List<ItemAmount>();

            // Iterate through our list of items
            foreach (var itemAmountInventory in itemList)
            {
                // For each item we want to take
                var took = false;
                foreach (var itemAmountWanted in itemsToTake)
                    // If the names match
                    if (itemAmountInventory.itemName == itemAmountWanted.itemName)
                    {
                        // If there is more than needed, leave the remaining
                        if (itemAmountInventory.amount > itemAmountWanted.amount)
                            toRemain.Add(new ItemAmount(itemAmountInventory.itemName,
                                itemAmountInventory.amount - itemAmountWanted.amount));

                        // Take what we need
                        toTake.Add(new ItemAmount(itemAmountWanted.itemName,
                            Mathf.Min(itemAmountWanted.amount, itemAmountInventory.amount)));
                        took = true;
                        break;
                    }

                if (!took)
                    toRemain.Add(itemAmountInventory);
            }

            itemList = toRemain;

            InventoryChange(itemList);
            return toTake;
        }

        // Take list of items from inventory, only take item
        // if inventory has sufficient amount
        public List<ItemAmount> TakeItemsExact(List<ItemAmount> amountToTake)
        {
            return null;
        }
    }
}