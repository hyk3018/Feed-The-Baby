using System;
using UnityEngine;

namespace FeedTheBaby.Commands
{
    public interface IInteractable
    {
        void Interact(GameObject interacter, Action<bool> onInteractFinish);
    }
}