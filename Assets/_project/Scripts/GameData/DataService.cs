using System;

namespace FeedTheBaby.GameData
{
    public static class DataService
    {
        public static IGameDataServiceFactory InstanceFactory { get; set; }

        public static IGameDataService Instance =>
            s_instance ?? (s_instance = InstanceFactory?.Build() ?? throw new InvalidOperationException());

        static IGameDataService s_instance;
    }
}