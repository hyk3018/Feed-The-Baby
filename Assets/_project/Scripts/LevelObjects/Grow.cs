using System;
using System.Collections;
using UnityEngine;

namespace FeedTheBaby.LevelObjects
{
    public class Grow : MonoBehaviour
    {
        [SerializeField] GrowthStage[] growthStages = null;
        [SerializeField] float totalGrowthTime = 5f;

        public bool FullyGrown => _currentStage == growthStages.Length - 1;

        SpriteRenderer _spriteRenderer;
        int _currentStage;
        int _totalGrowthProportions;
        float _currentGrowthTime;

        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            foreach (GrowthStage growthStage in growthStages)
                _totalGrowthProportions += growthStage.stageDurationProportion;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
                StartGrow();
        }

        public void StartGrow()
        {
            _currentStage = 0;
            _currentGrowthTime = 0;
            StartCoroutine(GrowUntilFull());
        }
        
        IEnumerator GrowUntilFull()
        {
            while (_currentStage < growthStages.Length - 1)
            {
                AddGrowth();
                yield return null;
            }
        }

        void AddGrowth()
        {
            _currentGrowthTime += Time.deltaTime;
            if (HasCrossedGrowthThreshold())
            {
                _currentStage += 1;
                CrossGrowthThreshold();
            }
        }

        void CrossGrowthThreshold()
        {
            if (_spriteRenderer)
            {
                _spriteRenderer.sprite = growthStages[_currentStage].sprite;
            }
        }

        bool HasCrossedGrowthThreshold()
        {
            int thresholdProportion = 0;
            for (int i = 0; i < _currentStage; i++)
                thresholdProportion += growthStages[i].stageDurationProportion;

            float growthPercentage = (float) thresholdProportion / _totalGrowthProportions;
            float thresholdTime = growthPercentage * totalGrowthTime;

            return _currentGrowthTime >= thresholdTime;
        }
    }

    [Serializable]
    public struct GrowthStage
    {
        public Sprite sprite;
        public int stageDurationProportion;
    }
}
