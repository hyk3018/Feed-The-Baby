using System;
using System.Linq;
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
        [SerializeField] LevelData[] levels = null;
        [SerializeField] int[] starsCollectedPerLevel = null;
        [SerializeField] SkillTreeData skillTreeData = null;
        [SerializeField] ItemUIData itemUIData = null;
        [SerializeField] LevelObjectMap levelObjectMap = null;
        [SerializeField] int currentLevel;
        [SerializeField] int levelsUnlocked = 1;
        [SerializeField] int starsSpent;

        void Awake()
        {
            skillTreeData.SetFirstTime();
            starsCollectedPerLevel = new int[levels.Length];
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

        public int GetStarsCollected() => starsCollectedPerLevel.Sum();
        public int GetStarsCollected(int level) => starsCollectedPerLevel[level];

        public void AddStarsForLevel(int collected, int level)
        {
            starsCollectedPerLevel[level] = Mathf.Max(collected, starsCollectedPerLevel[level]);
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

        public void ResetStars()
        {
            starsCollectedPerLevel = new int[levels.Length];
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
            data.ResetStars();
        }
    }
    #endif
}