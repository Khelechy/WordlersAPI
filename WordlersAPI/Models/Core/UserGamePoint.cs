namespace WordlersAPI.Models.Core
{
    public class UserGamePoint
    {
        public int Id { get; set; }
        public int GameId { get; set; } 
        public int UserId { get; set; }
        public int Point { get; set; }
    }
}
