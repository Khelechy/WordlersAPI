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
    }
}
