using Microsoft.AspNetCore.SignalR;
using WordlersAPI.Hubs;
using WordlersAPI.Interfaces;
using WordlersAPI.Models.Request;

namespace WordlersAPI.Services
{
    public class GameHubService : IGameHubService
    {
        private readonly IHubContext<GameHub> _gameHub;

        public GameHubService(IHubContext<GameHub> gameHub)
        {
            _gameHub = gameHub;
        }

        public async Task SendMessage(string roomId, string user, string message)
        {
           await _gameHub.Clients.Group(roomId).SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendRoundStatus(string roomId, bool status)
        {
            await _gameHub.Clients.Group(roomId).SendAsync("ReceiveRoundStatus", status);   
        }
    }
}
