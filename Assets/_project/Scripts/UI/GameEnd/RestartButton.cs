using UnityEngine;

namespace FeedTheBaby.UI
{
    public class RestartButton : MonoBehaviour
    {
        [SerializeField] SceneChanger sceneChanger = null;

        public void RestartLevel()
        {
            sceneChanger.ChangeTarget(SceneNumbers.LEVEL);
        }
    }
}