using System;
using UnityEngine;

namespace FeedTheBaby
{
    public class Pause : MonoBehaviour
    {
        [SerializeField] Animator pausePanelAnimator = null;
        static readonly int Show = Animator.StringToHash("Show");

        void Update()
        {
            if (LevelManager.Instance.playing && Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame();
            }
        }

        public void PauseGame()
        {
            pausePanelAnimator.SetTrigger(Show);
            LevelManager.Instance.Pause();
        }
    }
}