using WordlersAPI.Models.Core;
using WordlersAPI.Models.Request;

namespace WordlersAPI.Interfaces
{
    public interface IGameService
    {
        Task<Game> CreateGame(CreateGameRequestModel createGameRequest);
        Task<Game> StartGame(int gameId);
    }
}
