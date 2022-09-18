using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WordlersAPI.Data;
using WordlersAPI.Interfaces;
using WordlersAPI.Models.Constants;
using WordlersAPI.Models.Core;
using WordlersAPI.Models.DataModel;
using WordlersAPI.Models.Request;
using System.Timers;

namespace WordlersAPI.Services
{
    public class GameService : IGameService
    {
        private readonly ILogger<GameService> logger;
        private readonly GameDbContext context;
        private readonly IRabbitMQService rabbitMQService;

        public GameService(GameDbContext context, ILogger<GameService> logger, IRabbitMQService rabbitMQService)
        {
            this.logger = logger;
            this.context = context;
            this.rabbitMQService = rabbitMQService;
            
        }
        public async Task<Game> CreateGame(CreateGameRequestModel createGameRequest)
        {
            var game = new Game
            {
                Users = createGameRequest.Users,
                RoomId = createGameRequest.RoomId,
                RoundDuration = 60000
            };
            await context.Games.AddAsync(game);    
            await context.SaveChangesAsync();
            return game;
        }

        public async Task<Game> GetGame(int gameId)
        {
            return await context.Games.FirstOrDefaultAsync(x => x.Id == gameId);  
        }

        public async Task<Game> StartGameRound(int gameId)
        {
            var game = await context.Games.FirstOrDefaultAsync(x => x.Id == gameId);  
            game.InRound = true;

            //Handle Produce Start Game Round to Queue
            var gameMessage = new GameBrokerModel
            {
                GameId = gameId,
                RoundDuration = game.RoundDuration
            };
            var jsonMessage = JsonConvert.SerializeObject(gameMessage);
            await rabbitMQService.ProduceMessage(Constant.StartGameRoundTopic, jsonMessage);
            //End
            logger.LogInformation($".....................Game with id {gameId} has started round");
            await context.SaveChangesAsync();
            return game;
        }

        public async Task StopGameRound(int gameId)
        {
            var game = await context.Games.FirstOrDefaultAsync(x => x.Id == gameId);
            game.InRound = false;
            await context.SaveChangesAsync();
            logger.LogInformation($"........................Game with id {gameId} has ended round");
        }

        public async Task TimeGameRound(int gameId, int roundDuration)
        {
            System.Timers.Timer timer = new System.Timers.Timer(roundDuration);
            //Handle Produce Stop Game Round to Queue
            var gameMessage = new GameBrokerModel
            {
                GameId = gameId
            };
            var jsonMessage = JsonConvert.SerializeObject(gameMessage);
            timer.AutoReset = false;
            timer.Elapsed += async (sender, e) =>
            {
                timer.Dispose();    
                await rabbitMQService.ProduceMessage(Constant.StopGameRoundTopic, jsonMessage);
            };
            timer.Start();
        }
    }
}
