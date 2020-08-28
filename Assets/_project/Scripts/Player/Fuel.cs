using System;
using UnityEngine;

namespace FeedTheBaby.Player
{
    public class Fuel : MonoBehaviour
    {
        Timer _fuelTimer;

        void Awake()
        {
            _fuelTimer = GetComponent<Timer>();
            float playerStartTime = LevelManager.Instance.currentLevelData.playerStartTime;
            if (playerStartTime > 0)
            {
                LevelManager.Instance.LevelStart += (i) =>
                {
                    _fuelTimer.StartCount(playerStartTime);
                };
            }
            else
                enabled = false;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                AddFuel(3f);
        }

        public void AddFuel(float amount)
        {
            _fuelTimer.AddTime(amount);
        }
    }
}