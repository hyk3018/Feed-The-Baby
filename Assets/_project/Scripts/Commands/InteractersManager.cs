using System;
using System.Collections.Generic;
using UnityEngine;

namespace FeedTheBaby.Commands
{
    class InteractersManager : MonoBehaviour
    {
        [SerializeField] List<IInteracter> _interacters = new List<IInteracter>();

        // Get all components in gameobject that interacts
        void Awake()
        {
            var interacters = GetComponentsInChildren<IInteracter>();
            foreach (IInteracter interacter in interacters)
            {
                if (!_interacters.Contains(interacter))
                {
                    _interacters.Add(interacter);
                }
            }
        }
        
        public void Interact(Transform target, Action<bool> interactionEnd)
        {
            foreach (IInteracter interacter in _interacters)
            {
                interacter.Interact(target, interactionEnd);
            }
        }
    }
}