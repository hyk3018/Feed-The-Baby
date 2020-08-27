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
            float fuelAmount = LevelManager.Instance.currentLevelData.fuelAmount;
            if (fuelAmount > 0)
            {
                LevelManager.Instance.LevelStart += (i) =>
                {
                    _fuelTimer.StartCount(fuelAmount);
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