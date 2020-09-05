using UnityEngine;

namespace FeedTheBaby.UI
{
    public class BabyTimerUI : MonoBehaviour
    {
        void Awake()
        {
            if (LevelManager.Instance.currentLevelData.levelTime <= 0)
                gameObject.SetActive(false);
        }
    }
}