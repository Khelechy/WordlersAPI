using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using WordlersAPI.Interfaces;
using WordlersAPI.Models.Constants;
using WordlersAPI.Models.Request;

namespace WordlersAPI.Hubs
{
    public class GameHub : Hub<IGameHub>
    {
        private readonly IGameService _gameService;

        public GameHub(IGameService gameService)  
        {
            _gameService = gameService;
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message);
        }

        public async Task SendRoundStatus(string roomId, bool status)
        {
            await Clients.Group(roomId).ReceiveRoundStatus(status);
        }

        public async Task SendRoomMessage(string user, MessageRequestModel message)
        {
            await Clients.Group(message.RoomId).ReceiveMessage(user, message.MessageBody);
        }

        public async Task SendUserInputRoomMessage(string user, MessageRequestModel message)
        {

            await Clients.Group(message.RoomId).ReceiveMessage(user, message.MessageBody);
            //Proceed to User Game Input Logic
            var gameInputModel = new UserGameInputModel
            {
                GameId = message.GameId,
                OriginalWord = message.OriginalWord,
                UserId = message.UserId,
                UserName = message.UserName,
                UserWord = message.MessageBody,
                RoomId = message.RoomId
            };
            await _gameService.UserGameInput(gameInputModel);

        }

        public async Task AddToRoom(string roomId, string username, string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            await Clients.Group(roomId).ReceiveMessage(Constant.WordlerBotName, $"{username} has joined the room.");
        }

        public async Task RemoveFromRoom(string roomId, string username, string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

            await Clients.Group(roomId).ReceiveMessage(Constant.WordlerBotName, $"{username} has left the room.");
        }
    }
}
