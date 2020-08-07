using System;
using System.Collections.Generic;

namespace FeedTheBaby
{
    // Store data for picking up items and storing
    // them in inventory

    [Serializable]
    public class ItemTier
    {
        public List<ItemAmount> items = new List<ItemAmount>();

        public static ItemTier CollapseItemTiers(List<ItemTier> tieredItems)
        {
            var itemTier = new ItemTier();
            foreach (var tier in tieredItems) itemTier.AddItems(tier.items);

            return itemTier;
        }

        public static ItemTier CollapseAndCopyItemTiers(List<ItemTier> tieredItems)
        {
            var itemTier = new ItemTier();
            foreach (var tier in tieredItems) itemTier.AddItems(Copy(tier).items);

            return itemTier;
        }

        public static ItemTier Copy(ItemTier goalTier)
        {
            var copy = new ItemTier();
            foreach (var itemAmount in goalTier.items)
                copy.items.Add(new ItemAmount(itemAmount.itemName, itemAmount.amount));

            return copy;
        }


        public static ItemTier CopyZero(ItemTier goalTier)
        {
            var copy = new ItemTier();
            foreach (var itemAmount in goalTier.items) copy.items.Add(new ItemAmount(itemAmount.itemName, 0));

            return copy;
        }

        public void AddItem(ItemAmount itemToAdd)
        {
            var added = false;
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item.itemName == itemToAdd.itemName)
                {
                    items[i] = new ItemAmount(item.itemName, item.amount + itemToAdd.amount);
                    added = true;
                }
            }

            if (!added)
                items.Add(itemToAdd);
        }

        // Appends a list of item amounts to tier, and if items are the
        // same type, adds them into a single item amount
        public void AddItems(List<ItemAmount> itemsToAdd)
        {
            for (var i = 0; i < itemsToAdd.Count; i++) AddItem(itemsToAdd[i]);
        }

        public void RemoveItem(int i)
        {
            items.RemoveAt(i);
        }

        public static ItemTier[] CopyTiers(ItemTier[] goalTiers)
        {
            var newTiers = new ItemTier[goalTiers.Length];
            for (var i = 0; i < goalTiers.Length; i++) newTiers[i] = Copy(goalTiers[i]);

            return newTiers;
        }
    }

    [Serializable]
    public struct ItemAmount
    {
        public ItemType itemName;
        public int amount;

        public ItemAmount(int amount)
        {
            itemName = default;
            this.amount = amount;
        }

        public ItemAmount(ItemType name, int amount)
        {
            itemName = name;
            this.amount = amount;
        }

        public static void ZeroAmounts(List<ItemAmount> items)
        {
            for (var i = 0; i < items.Count; i++) items[i] = new ItemAmount(items[i].itemName, 0);
        }

        public static List<ItemAmount> AddList(List<ItemAmount> items1, List<ItemAmount> items2)
        {
            for (var i = 0; i < items2.Count; i++) Add(items1, items2[i]);

            return items1;
        }

        public static List<ItemAmount> Add(List<ItemAmount> items, ItemAmount item)
        {
            var added = false;
            for (var i = 0; i < items.Count; i++)
            {
                var itemInList = items[i];
                if (item.itemName == itemInList.itemName)
                {
                    itemInList.amount = itemInList.amount + item.amount;
                    items[i] = itemInList;
                    added = true;
                }
            }

            if (!added)
                items.Add(item);

            return items;
        }

        public static List<ItemAmount> Subtract(List<ItemAmount> items1, List<ItemAmount> items2)
        {
            for (var i = 0; i < items2.Count; i++)
            for (var j = 0; j < items1.Count; j++)
            {
                var alreadyAte = items1[j];
                if (items2[i].itemName == alreadyAte.itemName)
                {
                    alreadyAte.amount = alreadyAte.amount - items2[i].amount;
                    items1[j] = alreadyAte;
                }
            }

            return items1;
        }
    }

    public enum ItemType
    {
        Berry,
        Apple,
        Wood,
        Potato
    }
}