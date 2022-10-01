namespace WordlersAPI.Models.Request
{
    public class CreateGameRequestModel
    {
        public List<int> Users { get; set; }
        public string RoomId { get; set; }
    }
}
