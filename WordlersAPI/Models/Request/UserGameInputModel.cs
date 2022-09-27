namespace WordlersAPI.Models.Request
{
    public class UserGameInputModel
    {
        public int GameId { get; set; }
        public int UserId { get; set; }
        public string OriginalWord { get; set; }
        public string UserWord { get; set; }
    }
}
