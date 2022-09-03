using WordlersAPI.Data;
using WordlersAPI.Interfaces;
using WordlersAPI.Models.Core;
using WordlersAPI.Models.Request;

namespace WordlersAPI.Services
{
    public class GameService : IGameService
    {
        private readonly GameDbContext context;

        public GameService(GameDbContext context)
        {
            this.context = context;
        }
        public async Task<Game> CreateGame(CreateGameRequestModel createGameRequest)
        {
            var game = new Game
            {
                Users = createGameRequest.Users,
                RoomId = createGameRequest.RoomId,
            };
            await context.Games.AddAsync(game);    
            await context.SaveChangesAsync();
            return game;
        }

        public Task<Game> StartGame(int gameId)
        {
            throw new NotImplementedException();
        }
    }
}
