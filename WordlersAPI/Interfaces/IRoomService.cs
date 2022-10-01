using WordlersAPI.Models.Core;

namespace WordlersAPI.Interfaces
{
    public interface IRoomService
    {
        Task<Room> FindFreeRoom();
        Task<Room> CreateRoom();   
        Task<Room> IncrementAndUpdateRoomUsersCount(string roomId);
        Task PublishIncrementAndUpdateRoomUsersCount(string roomId);
        Task<Room> UserRequestRoom();
    }
}
