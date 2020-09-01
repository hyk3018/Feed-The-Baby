using System;
using UnityEngine;

namespace FeedTheBaby.Commands
{
    public interface IInteractor
    {
        void Interact(Transform target, Action<bool> interactionEnd);
        bool InteractsWith(Type type);
    }
}