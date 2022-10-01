namespace WordlersAPI.Models.Request
{
    public class MessageRequestModel
    {
        public string MessageBody { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string GameId { get; set; }
        public string RoomId { get; set; }
        public string OriginalWord { get; set; }
    }
}
