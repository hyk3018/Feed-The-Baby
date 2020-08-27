using System;
using UnityEngine;

namespace FeedTheBaby.UI
{
    public class FuelUI : MonoBehaviour
    {
        void Awake()
        {
            if (LevelManager.Instance.currentLevelData.fuelAmount <= 0)
                gameObject.SetActive(false);
        }
    }
}