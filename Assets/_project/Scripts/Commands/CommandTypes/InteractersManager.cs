using System;
using System.Collections.Generic;
using UnityEngine;

namespace FeedTheBaby.Commands
{
    class InteractersManager : MonoBehaviour
    {
        [SerializeField] List<IInteractor> _interacters = new List<IInteractor>();

        // Get all components in gameobject that interacts
        void Awake()
        {
            var interacters = GetComponentsInChildren<IInteractor>();
            foreach (IInteractor interacter in interacters)
            {
                if (!_interacters.Contains(interacter))
                {
                    _interacters.Add(interacter);
                }
            }
        }
        
        public void Interact(Transform target, Type interactableType, Action<bool> interactionEnd)
        {
            foreach (IInteractor interacter in _interacters)
            {
                if (interactableType == null || interacter.InteractsWith(interactableType))
                    interacter.Interact(target, interactionEnd);
            }
        }
    }
}