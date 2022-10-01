namespace WordlersAPI.Models.Core
{
    public class UserGamePoint
    {
        public int Id { get; set; }
        public string GameId { get; set; } 
        public string UserId { get; set; }
        public int Point { get; set; }
    }
}
