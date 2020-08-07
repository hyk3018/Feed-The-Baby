using FeedTheBaby.GameData;
using UnityEngine;

namespace FeedTheBaby.UI
{
    public class NextLevelButton : MonoBehaviour
    {
        [SerializeField] SceneChanger sceneChanger = null;

        public int nextLevel;

        public void GoNextLevel()
        {
            DataService.Instance.SetCurrentLevel(nextLevel);
            sceneChanger.ChangeTarget(SceneNumbers.LEVEL);
        }
    }
}