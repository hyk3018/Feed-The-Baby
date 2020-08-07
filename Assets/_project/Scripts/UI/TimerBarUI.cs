using UnityEngine;
using UnityEngine.UI;

namespace FeedTheBaby.UI
{
    internal class TimerBarUI : MonoBehaviour
    {
        [SerializeField] Image timerBar = null;
        [SerializeField] Timer timer = null;
        [SerializeField] RenderMode renderMode = RenderMode.ScreenSpaceOverlay;
        [SerializeField] bool activeOnStart = false;
        [SerializeField] bool destroyOnEnd = true;

        void Awake()
        {
            if (timer != null)
            {
                timer.TimerUpdate += UpdateUI;
                timer.TimerStart += ShowUI;

                if (destroyOnEnd)
                    timer.TimerEnd += DestroyTimer;
            }

            if (!activeOnStart)
            {
                GetComponent<Image>().enabled = false;
                timerBar.enabled = false;
            }
        }

        void Start()
        {
            if (timerBar)
                timerBar.fillAmount = 1;
        }

        void ShowUI(Timer t)
        {
            if (!activeOnStart)
            {
                if (renderMode == RenderMode.WorldSpace)
                    transform.SetParent(GameObject.FindWithTag("WorldCanvas").transform);

                GetComponent<Image>().enabled = true;
                timerBar.enabled = true;
            }
        }

        void UpdateUI(Timer t)
        {
            if (timer.Counting && timerBar)
                timerBar.fillAmount = timer.RemainingTime / timer.TimeToCount;
        }

        void DestroyTimer(Timer t)
        {
            timer.TimerStart -= ShowUI;
            timer.TimerUpdate -= UpdateUI;
            timer.TimerEnd -= DestroyTimer;

            Destroy(gameObject);
        }
    }
}