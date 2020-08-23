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

    public class MovePositionCommand : Command
    {
        public Vector2 target;

        public MovePositionCommand(Vector2 target)
        {
            this.target = target;
        }
    }
}