using System;
using FeedTheBaby.LevelObjects;
using UnityEngine;

namespace FeedTheBaby.Commands
{
    public class MoveAndInteractCommand : Command
    {
        public Transform target; 
        public Type interactableType;

        public MoveAndInteractCommand(Transform target, Type interactableType)
        {
            this.target = target;
            this.interactableType = interactableType;
        }
    }
}