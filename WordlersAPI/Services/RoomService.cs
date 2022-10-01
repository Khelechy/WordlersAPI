using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WordlersAPI.Data;
using WordlersAPI.Interfaces;
using WordlersAPI.Models.Constants;
using WordlersAPI.Models.Core;
using WordlersAPI.Models.DataModel;

namespace WordlersAPI.Services
{
    public class RoomService : IRoomService
    {
        private readonly ILogger<RoomService> logger;
        private readonly GameDbContext context;
        private readonly IRabbitMQService rabbitMQService;

        public RoomService(GameDbContext context, ILogger<RoomService> logger, IRabbitMQService rabbitMQService)
        {
            this.logger = logger;
            this.context = context;
            this.rabbitMQService = rabbitMQService;
        }

        public async Task<Room> CreateRoom()
        {
            var room = new Room
            {
                Name = DateTime.Now.ToString(),
                IsFull = false,
                UsersCount = 0,
            };
            await context.Rooms.AddAsync(room); 
            await context.SaveChangesAsync();   
            return room;
        }

        public async Task<Room> FindFreeRoom()
        {
            return await context.Rooms.FirstOrDefaultAsync(r => r.UsersCount < Constant.MaxRoomUsers || r.IsFull != true);   
        }

        public async Task<Room> IncrementAndUpdateRoomUsersCount(string roomId)
        {
            var room = await context.Rooms.FirstOrDefaultAsync(r => r.Id.ToString() == roomId);
            room.UsersCount += 1;
            if(room.UsersCount == Constant.MaxRoomUsers)
                room.IsFull = true; 
            context.Update(room);
            await context.SaveChangesAsync();
            return room;

        }

        public async Task PublishIncrementAndUpdateRoomUsersCount(string roomId)
        {
            var brokerModel = new RoomBrokerModel
            {
                RoomId = roomId,
            };
            var jsonBody = JsonConvert.SerializeObject(brokerModel);
            await rabbitMQService.ProduceMessage(Constant.IncrementRoomUsersCount, jsonBody);
        }

        public async Task<Room> UserRequestRoom()
        {
            var freeRoom = await FindFreeRoom();
            if(freeRoom != null)
            {
                return freeRoom;
            }
            var newRoom = await CreateRoom();
            return newRoom; 
        }
    }
}
