﻿using WordlersAPI.Models.Core;
using WordlersAPI.Models.Request;

namespace WordlersAPI.Interfaces
{
    public interface IGameService
    {
        Task<Game> GetGame(int gameId);
        Task<Game> CreateGame(CreateGameRequestModel createGameRequest);
        Task TimeGameRound(int gameId, int roundDuration);
        Task<Game> StartGameRound(int gameId);
        Task StopGameRound(int gameId); 
    }
}