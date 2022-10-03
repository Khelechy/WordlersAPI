namespace WordlersAPI.Interfaces
{
    public interface IGameHub
    {
        Task ReceiveMessage(string user, string message);
        Task ReceiveRoundStatus(bool status);
    }
}
