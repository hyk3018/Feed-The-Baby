using UnityEngine;

namespace FeedTheBaby.Commands
{
    public class MoveTransformCommand : Command
    {
        public Transform target;

        public MoveTransformCommand(Transform target)
        {
            this.target = target;
        }
    }
}