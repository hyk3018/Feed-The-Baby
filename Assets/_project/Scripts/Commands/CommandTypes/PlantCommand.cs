using UnityEngine;

namespace FeedTheBaby.Commands
{
    public class PlantCommand : MovePositionCommand
    {
        public PlantCommand(Vector2 position) : base(position)
        {
        }
    }
}