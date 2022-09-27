using WordlersAPI.Models.Core;

namespace WordlersAPI.Interfaces
{
    public interface IUserGamePointService
    {
        Task AddGamePoint(UserGamePoint userGamePoint);
        Task AddGamePointRange(List<UserGamePoint> userGamePoints);
    }
}
