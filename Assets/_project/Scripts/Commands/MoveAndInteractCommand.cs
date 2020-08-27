using UnityEngine;

namespace FeedTheBaby.Commands
{
    public class MoveAndInteractCommand : Command
    {
        public Transform target;

        public MoveAndInteractCommand(Transform target)
        {
            this.target = target;
        }
    }
}