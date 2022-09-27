using WordlersAPI.Data;
using WordlersAPI.Interfaces;
using WordlersAPI.Models.Core;

namespace WordlersAPI.Services
{
    public class UserGamePointService : IUserGamePointService
    {
        private readonly GameDbContext context;

        public UserGamePointService(GameDbContext context)
        {
            this.context = context;
        }
        public async Task AddGamePoint(UserGamePoint userGamePoint)
        {
            await context.AddAsync(userGamePoint);
            await context.SaveChangesAsync();   
        }

        public async Task AddGamePointRange(List<UserGamePoint> userGamePoints)
        {
            await context.AddRangeAsync(userGamePoints);
            await context.SaveChangesAsync();
        }
    }
}
