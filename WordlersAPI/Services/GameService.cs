using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WordlersAPI.Data;
using WordlersAPI.Interfaces;
using WordlersAPI.Models.Constants;
using WordlersAPI.Models.Core;
using WordlersAPI.Models.DataModel;
using WordlersAPI.Models.Request;
using System.Timers;
using WordlersAPI.Helpers;

namespace WordlersAPI.Services
{
    public class GameService : IGameService
    {
        private readonly ILogger<GameService> logger;
        private readonly ICacheService cacheService;
        private readonly GameDbContext context;
        private readonly IRabbitMQService rabbitMQService;
        private readonly IWordEngineService wordEngineService;

        public GameService(GameDbContext context, ILogger<GameService> logger, IRabbitMQService rabbitMQService, IWordEngineService wordEngineService, ICacheService cacheService)
        {
            this.logger = logger;
            this.cacheService = cacheService;
            this.context = context;
            this.rabbitMQService = rabbitMQService;
            this.wordEngineService = wordEngineService;


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

        public async Task UserGameInput(UserGameInputModel userGameInputModel)
        {
           

            //validate anagram
            var anagramRequest = new CheckAnagramRequestModel
            {
                OriginalWord = userGameInputModel.OriginalWord, 
                NewWord = userGameInputModel.UserWord
            };

            var isAnagram = wordEngineService.ValidateAnagram(anagramRequest);
            if(!isAnagram)
            {
                //ignore
                return;
            }

            //validate word
            var isValidWord = wordEngineService.ValidateWord(anagramRequest.NewWord);
            if (!isValidWord)
            {
                //ignore or prompt is not a valid word
                return;
            }

            //Check other users has not sent in word
            var isExist = await cacheService.IsStoreContains(Constant.GameStoreName + userGameInputModel.GameId.ToString(), anagramRequest.NewWord);
            if (isExist)
            {
                //Ignore or prompt has already been sent
                return;
            }
            else
            {
                await cacheService.AddToStore(Constant.GameStoreName + userGameInputModel.GameId.ToString(), anagramRequest.NewWord);
            }


            //generate point
            int point = PointGrader.AwardPoint(anagramRequest.NewWord);

            //save point
            var userGamePointMessage = new UserGamePoint
            {
                GameId = userGameInputModel.GameId,
                UserId = userGameInputModel.UserId,
                Point = point
            };
            var jsonMessage = JsonConvert.SerializeObject(userGamePointMessage);
            await rabbitMQService.ProduceMessage(Constant.StoreUserGamePoint, jsonMessage);

            //New Pipeline, return success message (username: you have got 2 for the word "word")
        }

        public async Task<bool> TestStore(string word)
        {
            return await cacheService.IsStoreContains("testkey", word);
        }

        public async Task<bool> AddStore(string word)
        {
            return await cacheService.AddToStore("testkey", word);
        }

        public async Task DeleteStore()
        {
            await cacheService.ClearStore("testkey");
        }
    }
}
