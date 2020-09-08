using System;
using UnityEngine;

namespace FeedTheBaby
{
    public class Timer : MonoBehaviour
    {
        public float TimeToCount { get; private set; }
        public float RemainingTime { get; private set; }
        public bool Counting { get; private set; }

        public Action<Timer> TimerStart;
        public Action<Timer> TimerUpdate;
        public Action<Timer> TimerEnd;

        void Awake()
        {
            LevelManager.Instance.LevelEnd += (success) => PauseTimer();
        }

        void Update()
        {
            if (Counting && LevelManager.Instance.playing)
            {
                RemainingTime -= Time.deltaTime;
                TimerUpdate?.Invoke(this);

                if (RemainingTime <= 0)
                {
                    Counting = false;
                    TimerEnd?.Invoke(this);
                }
            }
        }

        public void StartCount(float countTime)
        {
            TimeToCount = countTime;
            RemainingTime = countTime;
            Counting = true;
            
            TimerStart?.Invoke(this);
        }

        public void AddTime(float timeToAdd)
        {
            RemainingTime = Mathf.Min(RemainingTime + timeToAdd, TimeToCount);
        }

        void PauseTimer()
        {
            Counting = false;
        }

        public void RestartTimer()
        {
            Counting = true;
        }
    }
}