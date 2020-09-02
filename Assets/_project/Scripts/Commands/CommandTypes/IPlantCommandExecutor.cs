using System;

namespace FeedTheBaby.Commands
{
    public interface IPlantCommandExecutor
    {
        void ExecutePlant(PlantCommand plantCommand, Action<bool> onCommandFinish);
    }
}