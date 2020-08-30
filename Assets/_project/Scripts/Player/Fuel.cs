using System;
using UnityEngine;

namespace FeedTheBaby.Player
{
    public class Fuel : MonoBehaviour
    {
        [SerializeField] float timerPerFuel = 3f;
        
        Timer _fuelTimer;
        Inventory _inventory;

        void Awake()
        {
            _inventory = GetComponent<Inventory>();
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

        public void ConsumeFuel(int amount)
        {
            if (_inventory.SubtractFuel(amount))
                _fuelTimer.AddTime(amount * timerPerFuel);
        }
    }
}