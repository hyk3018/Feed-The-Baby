using System;
using UnityEngine;

namespace FeedTheBaby.Commands
{
    public interface IInteracter
    {
        void Interact(Transform target, Action<bool> interactionEnd);
    }
}