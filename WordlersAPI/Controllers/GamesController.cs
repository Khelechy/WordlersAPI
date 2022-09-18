using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WordlersAPI.Interfaces;
using WordlersAPI.Models.Request;

namespace WordlersAPI.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGameService gameService;

        public GamesController(IGameService gameService)
        {
            this.gameService = gameService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateGame(CreateGameRequestModel createGameRequestModel)
        {
            var game = await gameService.CreateGame(createGameRequestModel);    
            return Ok(game);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetGame(int id)
        {
            var game = await gameService.GetGame(id);
            return Ok(game);
        }

        [HttpGet("start/{id}")]
        public async Task<IActionResult> StartGame(int id)
        {
            var game = await gameService.StartGameRound(id);
            return Ok(game);
        }
    }
}
