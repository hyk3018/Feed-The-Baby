using UnityEngine;

namespace FeedTheBaby.Commands
{
    public class MoveAndInteractCommand : Command
    {
        public Transform target;
        public IInteractable interactable;

        public MoveAndInteractCommand(IInteractable interactable, Transform target)
        {
            this.interactable = interactable;
            this.target = target;
        }
    }
}