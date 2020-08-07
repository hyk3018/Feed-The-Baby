using System;
using FeedTheBaby.GameData;
using UnityEngine;
using UnityEngine.UI;

namespace FeedTheBaby.UI
{
    public class LevelSelectButton : MonoBehaviour
    {
        public int levelIndex;

        public void OpenLevelSelect()
        {
            DataService.Instance.SetCurrentLevel(levelIndex);
        }
    }
}