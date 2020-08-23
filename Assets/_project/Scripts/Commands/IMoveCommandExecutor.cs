using System;

namespace FeedTheBaby.Commands
{
    public interface IMoveCommandExecutor : IInterruptable
    {
        void ExecuteMoveTransform(MoveTransformCommand moveCommand, Action onCommandFinish);
        void ExecuteMovePosition(MovePositionCommand moveCommand, Action onCommandFinish);
    }

    public interface IInterruptable
    {
        void Interrupt();
    }
}