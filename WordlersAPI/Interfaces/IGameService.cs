using WordlersAPI.Models.Core;
using WordlersAPI.Models.Request;

namespace WordlersAPI.Interfaces
{
    public interface IGameService
    {
        Task<Game> GetGame(string gameId);
        Task<Game> CreateGame(CreateGameRequestModel createGameRequest);
        Task TimeGameRound(string gameId, int roundDuration);
        Task<Game> StartGameRound(string gameId);
        Task StopGameRound(string gameId);

        Task UserGameInput(UserGameInputModel userGameInputModel);


        Task<bool> AddStore(string word);
        Task<bool> TestStore(string word);
        Task DeleteStore();
    }
}
