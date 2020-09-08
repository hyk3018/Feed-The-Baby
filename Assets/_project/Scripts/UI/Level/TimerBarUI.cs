using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FeedTheBaby.UI
{
    internal class TimerBarUI : MonoBehaviour
    {
        [SerializeField] GameObject barObject = null;
        [SerializeField] Image barFill = null;
        [SerializeField] Timer timer = null;
        [SerializeField] TextMeshProUGUI timerCount = null;
        [SerializeField] RenderMode renderMode = RenderMode.ScreenSpaceOverlay;
        [SerializeField] bool activeOnStart = false;
        [SerializeField] bool destroyOnEnd = true;

        float _originalStartTime;

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
                barObject.SetActive(false);
                barFill.enabled = false;
            }
        }

        void Start()
        {
            if (barFill)
                barFill.fillAmount = 1;
        }

        void ShowUI(Timer t)
        {
            if (!activeOnStart)
            {
                if (renderMode == RenderMode.WorldSpace)
                    transform.SetParent(GameObject.FindWithTag("WorldCanvas").transform);

                barObject.SetActive(true);
                barFill.enabled = true;
            }
            
            if (timerCount)
                timerCount.text = Mathf.Ceil(timer.RemainingTime).ToString();

            _originalStartTime = timer.TimeToCount;
        }

        void UpdateUI(Timer t)
        {
            if (timer.Counting)
            {
                if (barFill)
                    barFill.fillAmount = Mathf.Round(Mathf.Min(1, timer.RemainingTime / _originalStartTime) * 50) / 50;
                if (timerCount)
                    timerCount.text = Mathf.Ceil(timer.RemainingTime).ToString();
            }
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