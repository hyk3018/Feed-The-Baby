using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FeedTheBaby
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = FindObjectOfType<GameManager>();

                    if (s_instance == null)
                    {
                        var singletonObject = new GameObject();
                        s_instance = singletonObject.AddComponent<GameManager>();
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return s_instance;
            }
        }

        static GameManager s_instance;

        [SerializeField] GameObject loading = null;
        [SerializeField] GameObject music = null;

        static List<AsyncOperation> _asyncOps;

        void Awake()
        {
            _asyncOps = new List<AsyncOperation>();

            if (loading == null)
            {
                loading = Instantiate(Resources.Load<GameObject>("Loading"));
                DontDestroyOnLoad(loading);
                DontDestroyOnLoad(music);
            }

            if (SceneManager.GetActiveScene().buildIndex == (int) SceneNumbers.PRELOAD)
                SceneManager.LoadSceneAsync((int) SceneNumbers.MAIN_MENU, LoadSceneMode.Additive);
        }

        public IEnumerator LoadScene(SceneNumbers currentScene, SceneNumbers sceneToLoad)
        {
            // Put up loading screen
            loading.SetActive(true);

            SceneManager.UnloadSceneAsync((int) currentScene);
            // Load required and wait for it to finish loading
            _asyncOps.Add(SceneManager.LoadSceneAsync((int) sceneToLoad, LoadSceneMode.Additive));
            StartCoroutine(WaitSceneLoad(currentScene));
            yield break;
        }

        IEnumerator WaitSceneLoad(SceneNumbers currentScene)
        {
            for (var i = 0; i < _asyncOps.Count; i++)
                while (!_asyncOps[i].isDone)
                    yield return null;

            // Once the required scene is loaded then unload current
            loading.SetActive(false);
        }
    }


    public enum SceneNumbers
    {
        PRELOAD = 0,
        MAIN_MENU = 1,
        LEVEL = 2
    }
}