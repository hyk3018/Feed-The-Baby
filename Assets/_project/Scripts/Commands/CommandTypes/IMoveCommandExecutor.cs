using System;

namespace FeedTheBaby.Commands
{
    public interface IMoveCommandExecutor : IInterruptable
    {
        void ExecuteMoveTransform(MoveTransformCommand moveCommand, Action<bool> onCommandFinish);
        void ExecuteMovePosition(MovePositionCommand moveCommand, Action<bool> onCommandFinish);
    }

    public interface IInterruptable
    {
        void Interrupt();
    }
}