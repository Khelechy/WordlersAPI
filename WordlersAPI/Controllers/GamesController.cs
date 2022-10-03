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
        public async Task<IActionResult> GetGame(string id)
        {
            var game = await gameService.GetGame(id);
            return Ok(game);
        }

        [HttpGet("start")]
        public async Task<IActionResult> StartGame([FromBody] StartGameRequest startGameRequest)
        {
            var game = await gameService.StartGameRound(startGameRequest.GameId, startGameRequest.RoomId);
            return Ok(game);
        }

        [HttpGet("game/store/add")]
        public async Task<IActionResult> AddToStore([FromQuery] string word)
        {
            var isAdded = await gameService.AddStore(word);
            return Ok(isAdded);
        }

        [HttpGet("game/store/contains")]
        public async Task<IActionResult> CheckStore([FromQuery] string word)
        {
            var isAdded = await gameService.TestStore(word);
            return Ok(isAdded);
        }

        [HttpPost("game/store/delete")]
        public async Task<IActionResult> Delete()
        {
            await gameService.DeleteStore();
            return Ok();
        }
    }
}
