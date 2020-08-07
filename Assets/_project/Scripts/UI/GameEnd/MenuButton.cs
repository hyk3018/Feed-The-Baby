using UnityEngine;

namespace FeedTheBaby.UI
{
    public class MenuButton : MonoBehaviour
    {
        [SerializeField] SceneChanger sceneChanger = null;

        public void GoToMenu()
        {
            sceneChanger.ChangeTarget(SceneNumbers.MAIN_MENU);
        }
    }
}