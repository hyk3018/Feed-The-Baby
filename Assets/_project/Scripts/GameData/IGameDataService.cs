using FeedTheBaby.LevelEditor;
using FeedTheBaby.SkillTree;
using FeedTheBaby.Tilemaps.Brushes;
using FeedTheBaby.UI;

namespace FeedTheBaby.GameData
{
    public interface IGameDataService
    {
        void SetCurrentLevel(int i);
        int GetCurrentLevel();
        LevelData[] GetLevels();
        SkillTreeData GetSkillTreeData();
        ItemUIData GetItemUIData();
        LevelObjectMap GetLevelObjectMap();
        int GetStarsCollected();
        int GetStarsCollected(int level);
        void SpendStars(int amount);
        void RestoreStars();
        int GetLevelsUnlocked();
        void UnlockNextLevel(int currentLevel);
        void AddStarsForLevel(int collected, int level);
    }
}