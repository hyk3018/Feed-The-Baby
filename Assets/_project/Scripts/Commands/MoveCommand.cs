using UnityEngine;

namespace FeedTheBaby.Commands
{
    public class MoveCommand : Command
    {
        public Vector2 target;

        public MoveCommand(Vector2 target)
        {
            this.target = target;
        }
    }
}