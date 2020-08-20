using System;

namespace FeedTheBaby.Commands
{
    public interface IMoveCommandExecutor : IInterruptable
    {
        void ExecuteMove(MoveCommand command, Action onCommandFinish);
    }

    public interface IInterruptable
    {
        void Interrupt();
    }
}