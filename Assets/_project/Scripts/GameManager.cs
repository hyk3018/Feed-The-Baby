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

        static GameObject loading = null;
        static GameObject music = null;

        static List<AsyncOperation> _asyncOps;
        AsyncOperation _loadSceneAsync;

        void Awake()
        {
            _asyncOps = new List<AsyncOperation>();

            if (loading == null)
            {
                loading = Instantiate(Resources.Load<GameObject>("Loading"));
                DontDestroyOnLoad(loading);
            }

            if (music == null)
            {
                music = Instantiate(Resources.Load<GameObject>("Music"));
                DontDestroyOnLoad(music);
            }
            
            if (SceneManager.GetActiveScene().buildIndex == (int) SceneNumbers.PRELOAD)
                SceneManager.LoadSceneAsync((int) SceneNumbers.MAIN_MENU, LoadSceneMode.Additive);
        }

        public IEnumerator LoadScene(SceneNumbers currentScene, SceneNumbers sceneToLoad)
        {
            // Put up loading screen
            loading.SetActive(true);
            
            // Load required and wait for it to finish loading
            _loadSceneAsync = SceneManager.LoadSceneAsync((int) sceneToLoad);
            _asyncOps.Add(_loadSceneAsync);
            _loadSceneAsync.allowSceneActivation = false;
            StartCoroutine(WaitSceneLoad(currentScene));
            yield break;
        }

        IEnumerator WaitSceneLoad(SceneNumbers currentScene)
        {
            while (!_loadSceneAsync.isDone)
            {
                if (_loadSceneAsync.progress >= 0.9f)
                {
                    // Once the required scene is loaded then unload current
                    loading.SetActive(false);
                    _loadSceneAsync.allowSceneActivation = true;
                }

                yield return null;
            }

        }
    }


    public enum SceneNumbers
    {
        PRELOAD = 0,
        MAIN_MENU = 1,
        LEVEL = 2
    }
}