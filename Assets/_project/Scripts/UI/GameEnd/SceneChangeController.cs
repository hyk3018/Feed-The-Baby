using UnityEngine;

namespace FeedTheBaby.UI
{
    public class SceneChangeController : MonoBehaviour
    {
        [SerializeField] SceneChanger sceneChanger = null;
        [SerializeField] SceneNumbers sceneTarget = default;

        public void ChangeTarget()
        {
            sceneChanger.ChangeTarget(sceneTarget);
        }
    }
}