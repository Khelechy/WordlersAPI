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
        private readonly IGameHubService gameHubService;

        public GameService(GameDbContext context, ILogger<GameService> logger, IRabbitMQService rabbitMQService, 
            IWordEngineService wordEngineService, ICacheService cacheService, IGameHubService gameHubService)
        {
            this.logger = logger;
            this.cacheService = cacheService;
            this.context = context;
            this.rabbitMQService = rabbitMQService;
            this.wordEngineService = wordEngineService;
            this.gameHubService = gameHubService;


        }
        public async Task<Game> CreateGame(CreateGameRequestModel createGameRequest)
        {
            var game = new Game
            {
                Users = createGameRequest.Users,
                RoomId = createGameRequest.RoomId,
                RoundDuration = 60000,
                TimeInBetweenRound = 20000,
                NumberOfRounds = Constant.MaxNumberOfRounds
            };
            await context.Games.AddAsync(game);    
            await context.SaveChangesAsync();
            return game;
        }

        public async Task<Game> GetGame(string gameId)
        {
            return await context.Games.FirstOrDefaultAsync(x => x.Id.ToString() == gameId);  
        }

        public async Task<Game> StartGameRound(string gameId, string roomId)
        {
            var game = await context.Games.FirstOrDefaultAsync(x => x.Id.ToString() == gameId);  
            game.InRound = true;

            //Handle Produce Start Game Round to Queue
            var gameMessage = new GameBrokerModel
            {
                GameId = gameId,
                RoundDuration = game.RoundDuration,
                RoomId = roomId   
               
            };
            var jsonMessage = JsonConvert.SerializeObject(gameMessage);
            await rabbitMQService.ProduceMessage(Constant.StartGameRoundTopic, jsonMessage);
            //End
            logger.LogInformation($".....................Game with id {gameId} has started round");
            await context.SaveChangesAsync();
            //Send Round Start Event
            await gameHubService.SendRoundStatus(roomId, true);
            //
            await gameHubService.SendMessage(roomId, Constant.WordlerBotName, $"Game round has started");

            return game;
        }

        public async Task StopGameRound(string gameId, string roomId)
        {
            var game = await context.Games.FirstOrDefaultAsync(x => x.Id.ToString() == gameId);
            game.InRound = false;
            await context.SaveChangesAsync();
            //Send Round End Event
            await gameHubService.SendRoundStatus(roomId, false);
            //
            await gameHubService.SendMessage(roomId, Constant.WordlerBotName, $"This round has ended");
            logger.LogInformation($"........................Game with id {gameId} has ended round");

            //check If there are more rounds
            //Create a buffer time to inform the users
            //Trigger a new StartGame round session
            var counter = await cacheService.GetCounter(Constant.GameCounterName + gameId);
            if(counter < game.NumberOfRounds)
            {
                await gameHubService.SendMessage(roomId, Constant.WordlerBotName, $"The next round will begin soon");
                await TimeInBetweenRound(gameId, roomId, game.TimeInBetweenRound);
            }

        }

        public async Task TimeInBetweenRound(string gameId, string roomId, int roundDuration)
        {
            System.Timers.Timer timer = new System.Timers.Timer(roundDuration);
            //Handle Produce Stop Game Round to Queue
            var gameMessage = new GameBrokerModel
            {
                GameId = gameId,
                RoomId = roomId,
            };
            var jsonMessage = JsonConvert.SerializeObject(gameMessage);
            timer.AutoReset = false;
            timer.Elapsed += async (sender, e) =>
            {
                timer.Dispose();
                await rabbitMQService.ProduceMessage(Constant.StartNewGameRoundTopic, jsonMessage);
            };
            timer.Start();
        }

        public async Task TimeGameRound(string gameId, string roomId, int roundDuration)
        {
            System.Timers.Timer timer = new System.Timers.Timer(roundDuration);
            //Handle Produce Stop Game Round to Queue
            var gameMessage = new GameBrokerModel
            {
                GameId = gameId,
                RoomId = roomId,
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
            var isExist = await cacheService.IsStoreContains(Constant.GameStoreName + userGameInputModel.GameId, anagramRequest.NewWord);
            if (isExist)
            {
                //Ignore or prompt has already been sent
                return;
            }
            else
            {
                await cacheService.AddToStore(Constant.GameStoreName + userGameInputModel.GameId, anagramRequest.NewWord);
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
            await gameHubService.SendMessage(userGameInputModel.RoomId, Constant.WordlerBotName, $"{userGameInputModel.UserName}, You have been awarded {point} points for the word {userGameInputModel.UserWord}");
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
