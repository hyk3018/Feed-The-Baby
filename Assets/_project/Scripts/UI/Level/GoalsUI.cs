using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FeedTheBaby.UI
{
    public class GoalsUI : MonoBehaviour
    {
        [SerializeField] ItemUIData itemUiData = null;
        [SerializeField] Goals goals = null;
        [SerializeField] GameObject[] goalTiersContainer = null;
        [SerializeField] GameObject goalTemplate = null;

        readonly List<List<GameObject>> _goalSlotTiers = new List<List<GameObject>>();
        List<ItemTier> _goalTiers;

        void Start()
        {
            SetupUI(goals.GetGoals());
        }

        void SetupUI(List<ItemTier> goalTiers)
        {
            _goalTiers = goalTiers;

            for (var i = 0; i < goalTiers.Count; i++)
            {
                var goalSlotTier = new List<GameObject>();
                var tier = goalTiers[i].items;
                for (var j = 0; j < tier.Count; j++)
                {
                    var goalSlot = Instantiate(goalTemplate, goalTiersContainer[i].transform);
                    goalSlot.transform.localPosition = new Vector3(j * 25 - 50, 0, 0);

                    var images = goalSlot.GetComponentsInChildren<Image>();
                    images[1].fillAmount = 0;
                    images[2].sprite = itemUiData.itemSpriteDictionary.dictionary[tier[j].type];

                    var text = goalSlot.GetComponentInChildren<TextMeshProUGUI>();
                    text.text = "0/" + tier[j].amount;

                    goalSlotTier.Add(goalSlot);
                }

                _goalSlotTiers.Add(goalSlotTier);
            }

            goals.GoalsChanged += OnGoalsChanged;
        }

        void OnGoalsChanged(ItemTier[] hunger)
        {
            for (var i = 0; i < _goalTiers.Count; i++)
            {
                var tier = _goalTiers[i].items;
                for (var j = 0; j < tier.Count; j++)
                {
                    var goalSlot = _goalSlotTiers[i][j];

                    var images = goalSlot.GetComponentsInChildren<Image>();
                    images[1].fillAmount = (float) hunger[i].items[j].amount / tier[j].amount;

                    var text = goalSlot.GetComponentInChildren<TextMeshProUGUI>();
                    text.text = hunger[i].items[j].amount + "/" + tier[j].amount;
                }
            }
        }
    }
}