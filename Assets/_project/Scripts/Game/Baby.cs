using System;
using System.Collections;
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
        [SerializeField] AnimationClip[] emotionAnimations = null;

        public Action<List<ItemAmount>> HungerChanged;

        static readonly int IsSad = Animator.StringToHash("isSad");
        static readonly int IsHappy = Animator.StringToHash("isHappy");

        Feeder _feeder;

        List<ItemAmount> _currentHungerFilled;
        List<ItemAmount> _remainingHunger;
        bool _showingEmotion;
        float _emotionDuration = 5f;

        void Awake()
        {
            LevelManager.Instance.LevelStart += Setup;
            LevelManager.Instance.LevelEnd += (success) =>
            {
                if (success)
                    ShowHappy();
                else
                    ShowSad();
            };
            
            if (emotionAnimations != null && emotionAnimations.Length > 0)
                _emotionDuration = emotionAnimations.Max(t => t.length);
            
        }

        // For each item we are eating, subtract the amount 
        // away from the hunger for that item
        public void Feed(List<ItemAmount> foods)
        {
            if (foods == null || foods.Count == 0)
            {
                ShowSad();
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

            goals.TierFilled += i => ShowHappy();
        }

        void ShowHappy()
        {
            if (!_showingEmotion)
            {
                Debug.Log("Yes happy");
                _showingEmotion = true;
                CameraSound.PlaySoundAtCameraDepth(happySound, transform.position, 0.5f);
                emotionAnimator.SetTrigger(IsHappy);
                StartCoroutine(FinishShowingEmotion());
            }
        }

        void ShowSad()
        {
            if (!_showingEmotion)
            {
                _showingEmotion = true;
                CameraSound.PlaySoundAtCameraDepth(cryingSound, transform.position, 0.5f);
                emotionAnimator.SetTrigger(IsSad);
                StartCoroutine(FinishShowingEmotion());
            }
        }

        IEnumerator FinishShowingEmotion()
        {
            yield return new WaitForSeconds(_emotionDuration);
            _showingEmotion = false;
        }

        public CommandType PossibleCommands() => CommandType.FEED;
    }
}