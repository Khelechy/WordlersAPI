namespace WordlersAPI.Models.Request
{
    public class CheckAnagramRequestModel
    {
        public string OriginalWord { get; set; }
        public string NewWord { get; set; }
    }
}
