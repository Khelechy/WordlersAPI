using WordlersAPI.Interfaces;

namespace WordlersAPI.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        public Task ProduceMessage(string topic, string message)
        {
            throw new NotImplementedException();
        }
    }
}
