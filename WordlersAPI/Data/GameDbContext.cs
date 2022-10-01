using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WordlersAPI.Models.Core;

namespace WordlersAPI.Data
{
    public class GameDbContext : IdentityDbContext<User>
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
             
        }

        public new DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<UserGamePoint> UserGamePoints { get; set; }


    }
}
