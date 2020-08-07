using System;
using System.Collections;
using FeedTheBaby.GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FeedTheBaby.UI
{
    public class GameEndUI : MonoBehaviour
    {
        [SerializeField] GameObject stars = null;
        [SerializeField] TextMeshProUGUI endGameMessage = null;
        [SerializeField] GameObject collectedStarPrefab = null;
        [SerializeField] GameObject uncollectedStarPrefab = null;
        [SerializeField] Goals goals = null;
        [SerializeField] GameObject nextLevelButton = null;

        static readonly int Show = Animator.StringToHash("Show");

        Animator _animator;
        static readonly int Disabled = Animator.StringToHash("Disabled");

        void Awake()
        {
            _animator = GetComponent<Animator>();
            LevelManager.Instance.GameEnd += () => StartCoroutine(ShowPanel());
        }

        IEnumerator ShowPanel()
        {
            StartCoroutine(UpdatePanelData());
            yield return new WaitForSeconds(2f);
            _animator.SetTrigger(Show);
        }

        IEnumerator UpdatePanelData()
        {
            var currentLevel = DataService.Instance.GetCurrentLevel();
            if (DataService.Instance.GetLevelsUnlocked() > currentLevel + 1)
            {
                nextLevelButton.GetComponent<NextLevelButton>().nextLevel = currentLevel + 1;
                nextLevelButton.GetComponent<Button>().enabled = true;
            }
            else
            {
                nextLevelButton.GetComponent<Button>().enabled = false;
                nextLevelButton.GetComponent<Animator>().SetTrigger(Disabled);
            }

            foreach (Transform child in stars.transform)
                Destroy(child.gameObject);

            var collectedStars = goals.CollectedStars;
            var availableStars = goals.AvailableStars;

            for (var i = 0; i < availableStars; i++)
                Instantiate(i < collectedStars ? collectedStarPrefab : uncollectedStarPrefab, stars.transform);

            if (goals.CollectedStars == goals.AvailableStars)
                endGameMessage.text = "Congrats! You've collected all available stars for this level!";
            else
                endGameMessage.text =
                    $"You've collected {collectedStars} out of {availableStars} stars for this level!";

            yield break;
        }
    }
}