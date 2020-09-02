using System;
using FeedTheBaby.Commands;
using UnityEngine;

namespace FeedTheBaby.Player
{
    public class Planter : MonoBehaviour, IPlantCommandExecutor
    {
        [SerializeField] GameObject fuelPrefab = null;
        [SerializeField] Transform plantsHolder = null;

        public void ExecutePlant(PlantCommand plantCommand, Action<bool> onCommandFinish)
        {
            float x = Mathf.Floor(plantCommand.target.x) + 0.5f;
            float y = Mathf.Floor(plantCommand.target.y) + 0.5f;
            GameObject fuelObject = Instantiate(fuelPrefab, plantsHolder);
            fuelObject.transform.position = new Vector3(x, y, fuelObject.transform.position.z);
            onCommandFinish(true);
        }
    }
}