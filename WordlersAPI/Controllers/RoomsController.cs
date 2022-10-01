using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WordlersAPI.Interfaces;

namespace WordlersAPI.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService roomService;

        public RoomsController(IRoomService roomService)
        {
            this.roomService = roomService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom()
        {
            var room = await roomService.CreateRoom();
            return Ok(room);
        }

        [HttpGet("free")]
        public async Task<IActionResult> FindFreeRoom()
        {
            var room = await roomService.FindFreeRoom();
            return Ok(room);
        }

        [HttpGet("request")]
        public async Task<IActionResult> RequestRoom()
        {
            var room = await roomService.UserRequestRoom();
            return Ok(room);
        }

        [HttpPost("increment/{id}")]
        public async Task<IActionResult> IncrementRoomUsersCount(string id)
        {
            await roomService.PublishIncrementAndUpdateRoomUsersCount(id);
            return Ok();
        }
    }
}
