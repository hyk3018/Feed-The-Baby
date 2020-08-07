namespace FeedTheBaby.GameData
{
    public interface IGameDataServiceFactory
    {
        IGameDataService Build();
    }
}