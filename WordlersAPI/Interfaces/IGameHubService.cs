using WordlersAPI.Models.Request;

namespace WordlersAPI.Interfaces
{
    public interface IGameHubService
    {
        Task SendMessage(string roomId, string user, string message);
        Task SendRoundStatus(string roomId, bool status);
    }
}
