using System;
using System.Collections.Generic;
using System.Linq;
using FeedTheBaby.Commands;
using FeedTheBaby.LevelObjects;
using FeedTheBaby.Player;
using UnityEngine;

namespace FeedTheBaby.Game
{
    // Player interacts with this to feed baby and
    // beat the level

    public interface IFeedable : IInteractable
    {
        void Feed(List<ItemAmount> foods);
        List<ItemAmount> GetHunger();
    }

    public class Baby : MonoBehaviour, IFeedable
    {
        [SerializeField] Animator emotionAnimator = null;
        [SerializeField] AudioClip happySound = null;
        [SerializeField] public AudioClip cryingSound = null;

        public Action<List<ItemAmount>> HungerChanged;

        static readonly int IsSad = Animator.StringToHash("isSad");
        static readonly int IsHappy = Animator.StringToHash("isHappy");

        Feeder _feeder;

        List<ItemAmount> _currentHungerFilled;
        List<ItemAmount> _remainingHunger;

        void Awake()
        {
            LevelManager.Instance.LevelStart += Setup;
            LevelManager.Instance.EndWithStarsUncollected += () =>
            {
                AudioSource.PlayClipAtPoint(cryingSound, Camera.main.transform.position, 0.1f);
                emotionAnimator.SetTrigger(IsSad);
            };
        }

        // For each item we are eating, subtract the amount 
        // away from the hunger for that item
        public void Feed(List<ItemAmount> foods)
        {
            if (foods == null || foods.Count == 0)
            {
                AudioSource.PlayClipAtPoint(cryingSound, transform.position);
                emotionAnimator.SetTrigger(IsSad);
                return;
            }

            // Add foods to hunger filled and subtract from remaning
            _currentHungerFilled = ItemAmount.AddList(_currentHungerFilled, foods);
            _remainingHunger = ItemAmount.Subtract(_remainingHunger, foods);

            HungerChanged(_currentHungerFilled);
        }

        public List<ItemAmount> GetHunger()
        {
            return _remainingHunger;
        }

        // Used to initialise the baby's hunger
        // Set each sub-goal's item amounts to 0 at the start
        void SetHunger(Goals goals)
        {
            _remainingHunger = goals.GetCollapsedGoals();
            _currentHungerFilled = _remainingHunger.ToList();
            ItemAmount.ZeroAmounts(_currentHungerFilled);
        }

        void Setup(Goals goals)
        {
            SetHunger(goals);

            goals.TierFilled += i =>
            {
                AudioSource.PlayClipAtPoint(happySound, Camera.main.transform.position, 0.1f);
                emotionAnimator.SetTrigger(IsHappy);
            };
        }

        public CommandType PossibleCommands() => CommandType.FEED;
    }
}