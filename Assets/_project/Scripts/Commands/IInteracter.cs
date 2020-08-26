using System;

namespace FeedTheBaby.Commands
{
    public interface IInteracter
    {
        void Interact(IInteractable interactable, Action<bool> interactionEnd);
    }
}