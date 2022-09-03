using Microsoft.EntityFrameworkCore;
using WordlersAPI.Models.Core;

namespace WordlersAPI.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
             
        }

        public DbSet<Game> Games { get; set; }  
        public DbSet<UserGamePoint> UserGamePoints { get; set; }


    }
}
