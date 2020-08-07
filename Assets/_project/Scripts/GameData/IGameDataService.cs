using FeedTheBaby.Brushes;
using FeedTheBaby.Editor.Brushes;
using FeedTheBaby.LevelEditor;
using FeedTheBaby.SkillTree;
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
        void AddStars(int amount);
        void SpendStars(int amount);
        void RestoreStars();
        int GetLevelsUnlocked();
        void UnlockNextLevel(int currentLevel);
    }
}