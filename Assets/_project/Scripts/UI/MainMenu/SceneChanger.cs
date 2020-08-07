using UnityEngine;

namespace FeedTheBaby.UI
{
    public class SceneChanger : MonoBehaviour
    {
        [SerializeField] SceneNumbers currentScene = default;
        [SerializeField] SceneNumbers sceneToChangeInto = default;

        public void ChangeScene()
        {
            StartCoroutine(
                GameManager.Instance.LoadScene(currentScene, sceneToChangeInto));
        }

        public void ChangeTarget(SceneNumbers targetScene)
        {
            sceneToChangeInto = targetScene;
        }
    }
}