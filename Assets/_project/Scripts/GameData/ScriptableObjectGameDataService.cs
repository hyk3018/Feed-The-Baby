using System;
using UnityEngine;

namespace FeedTheBaby.GameData
{
    public class ScriptableObjectGameDataService : MonoBehaviour, IGameDataServiceFactory
    {
        [SerializeField] GameData gameDataScriptableObject = null;

        void Awake()
        {
            DataService.InstanceFactory = DataService.InstanceFactory ?? this;
        }

        public IGameDataService Build()
        {
            return gameDataScriptableObject;
        }
    }
}