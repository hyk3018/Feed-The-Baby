using System;
using System.Collections.Generic;
using System.Linq;
using FeedTheBaby.Game;
using UnityEngine;

namespace FeedTheBaby
{
    public class Goals : MonoBehaviour
    {
        [SerializeField] Baby baby = null;

        public Action<ItemTier[]> GoalsChanged;
        public Action<int> TierFilled;
        public Action FinalTierFilled;

        ItemTier[] _goalTiers;
        bool[] _tiersFilled;
        int _finalTier;
        public int AvailableStars { get; private set; }
        public int CollectedStars { get; private set; }

        void Awake()
        {
            _goalTiers = LevelManager.Instance.currentLevelData.goals;
            for (var i = 0; i < _goalTiers.Length; i++)
                if (_goalTiers[i].items.Count > 0)
                {
                    _finalTier = i;
                    AvailableStars = i + 1;
                }

            _tiersFilled = new bool[_goalTiers.Length];
        }

        void Start()
        {
            baby.HungerChanged += UpdateGoals;
        }

        // We get a list total of amount of hunger filled and use that
        // to determine whether we have reached the goals
        void UpdateGoals(List<ItemAmount> items)
        {
            var filled = items.ToList();

            // Fill the goals up
            FillGoals(filled, out var currentFilled, out var newTiersFilled);

            // Inform goals changed and also detect if tier reached since last time
            GoalsChanged?.Invoke(currentFilled);
            for (var i = 0; i < _tiersFilled.Length; i++)
                if (newTiersFilled[i] && !_tiersFilled[i])
                {
                    _tiersFilled[i] = i == 0 || _tiersFilled[i - 1];
                    if (_tiersFilled[i])
                    {
                        CollectedStars += 1;
                        TierFilled?.Invoke(i);
                    }
                }

            if (_tiersFilled[_finalTier])
                FinalTierFilled();
        }

        void FillGoals(List<ItemAmount> filled, out ItemTier[] currentFilled, out bool[] newTiersFilled)
        {
            currentFilled = new ItemTier[_goalTiers.Length];
            newTiersFilled = new bool[_goalTiers.Length];

            for (var i = 0; i < _goalTiers.Length; i++)
            {
                var goalTier = _goalTiers[i];

                if (goalTier.items.Count == 0)
                    break;
                currentFilled[i] = ItemTier.CopyZero(goalTier);
                var goalTierItems = goalTier.items;
                var filledTier = true;

                // For each tier we need to check if each item is filled
                for (var j = 0; j < goalTierItems.Count; j++)
                {
                    var filledItem = FillTier(filled, currentFilled, goalTierItems, j, i);

                    if (!filledItem)
                        filledTier = false;
                }

                newTiersFilled[i] = filledTier;
            }
        }

        static bool FillTier(List<ItemAmount> filled, ItemTier[] currentFilled, List<ItemAmount> goalTierItems, int j,
            int i)
        {
            var filledItem = true;

            // Compare with items we have filled
            for (var k = 0; k < filled.Count; k++)
            {
                var filledAmount = filled[k];
                if (goalTierItems[j].type == filledAmount.type)
                {
                    // If what we need is greater, this item is not filled
                    if (goalTierItems[j].amount > filledAmount.amount) filledItem = false;

                    // Set the fill to be min of the two amounts
                    currentFilled[i].items[j] = new ItemAmount(filledAmount.type,
                        Mathf.Min(goalTierItems[j].amount, filledAmount.amount));

                    // Subtract needed from filled
                    filledAmount.amount = Mathf.Max(0, filledAmount.amount - goalTierItems[j].amount);
                    filled[k] = filledAmount;

                    break;
                }
            }

            return filledItem;
        }

        public List<ItemTier> GetGoals()
        {
            return _goalTiers.ToList();
        }

        public List<ItemAmount> GetCollapsedGoals()
        {
            return ItemTier.CollapseAndCopyItemTiers(_goalTiers.ToList()).items;
        }
    }
}