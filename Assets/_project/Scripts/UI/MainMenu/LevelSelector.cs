using System;
using FeedTheBaby.GameData;
using UnityEngine;
using UnityEngine.UI;

namespace FeedTheBaby.UI
{
    public interface ILevelSelector
    {
        void SelectLevel();
        void SetLevelToSelect(int level);
    }

    public class LevelSelector : MonoBehaviour, ILevelSelector
    {
        [SerializeField] bool showLevelUI = false;
        
        public FocusedLevelUI focusedLevelUI;
        int _levelIndex;

        public void SelectLevel()
        {
            DataService.Instance.SetCurrentLevel(_levelIndex);
            
            if (showLevelUI)
                ShowFocusedLevelUI();
        }

        public void SetLevelToSelect(int level)
        {
            _levelIndex = level;
        }

        void ShowFocusedLevelUI()
        {
            StartCoroutine(focusedLevelUI.FetchLevelData());
            focusedLevelUI.ShowUI();
        }
    }
}