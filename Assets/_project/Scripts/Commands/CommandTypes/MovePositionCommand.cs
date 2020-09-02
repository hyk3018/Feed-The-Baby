using UnityEngine;

namespace FeedTheBaby.Commands
{
    public class MovePositionCommand : Command
    {
        public Vector2 target;

        public MovePositionCommand(Vector2 target)
        {
            this.target = target;
        }
    }
}