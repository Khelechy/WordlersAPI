namespace WordlersAPI.Models.Response
{
    public class GeneralResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class GeneralResponse<T>
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
    }
}
