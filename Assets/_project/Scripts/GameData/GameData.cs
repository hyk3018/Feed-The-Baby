using System;
using FeedTheBaby.LevelEditor;
using FeedTheBaby.SkillTree;
using FeedTheBaby.Tilemaps.Brushes;
using FeedTheBaby.UI;
using UnityEditor;
using UnityEngine;

namespace FeedTheBaby.GameData
{
    [CreateAssetMenu(fileName = "Game Data", menuName = "Feed The Baby/Game Data", order = 0)]
    public class GameData : ScriptableObject, IGameDataService
    {
        // [SerializeField] LevelData level = null;
        [SerializeField] LevelData[] levels = null;
        [SerializeField] SkillTreeData skillTreeData = null;
        [SerializeField] ItemUIData itemUIData = null;
        [SerializeField] LevelObjectMap levelObjectMap = null;
        [SerializeField] int currentLevel;
        [SerializeField] int levelsUnlocked = 1;
        int _starsCollected;
        int _starsSpent;

        void Awake()
        {
            skillTreeData.SetFirstTime();
        }

        public void SetCurrentLevel(int i)
        {
            currentLevel = i;
        }

        public int GetCurrentLevel()
        {
            return currentLevel;
        }

        public LevelData[] GetLevels()
        {
            return levels;
        }

        public SkillTreeData GetSkillTreeData()
        {
            return skillTreeData;
        }

        public ItemUIData GetItemUIData()
        {
            return itemUIData;
        }

        public LevelObjectMap GetLevelObjectMap()
        {
            return levelObjectMap;
        }

        public int GetStarsCollected()
        {
            return _starsCollected;
        }

        public void AddStars(int amount)
        {
            _starsCollected += amount;
        }

        public void SpendStars(int amount)
        {
            throw new NotImplementedException();
        }

        public void RestoreStars()
        {
            throw new NotImplementedException();
        }

        public int GetLevelsUnlocked()
        {
            return levelsUnlocked;
        }

        public void UnlockNextLevel(int currentLevel)
        {
            levelsUnlocked = Mathf.Min(Mathf.Max(levelsUnlocked, currentLevel + 2),
                levels.Length);
        }

        public void ResetLevel()
        {
            currentLevel = 0;
            levelsUnlocked = 1;
        }
    }

    #if UNITY_EDITOR
    public class ResetGame : MonoBehaviour
    {
        [MenuItem("Feed The Baby/Reset Game")]
        static void Reset()
        {
            var data = Resources.Load<GameData>("Game Data");
            data.ResetLevel();
        }
    }
    #endif
}