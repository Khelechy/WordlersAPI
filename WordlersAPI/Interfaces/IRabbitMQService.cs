namespace WordlersAPI.Interfaces
{
    public interface IRabbitMQService
    {
        Task ProduceMessage(string topic, string message);
    }
}
