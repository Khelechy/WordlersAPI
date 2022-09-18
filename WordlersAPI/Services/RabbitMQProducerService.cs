using RabbitMQ.Client;
using System.Text;
using WordlersAPI.Interfaces;

namespace WordlersAPI.Services
{
    public class RabbitMQProducerService : IRabbitMQService
    {
        private readonly IConfiguration configuration;
        ConnectionFactory factory;
        IConnection connection;
        IModel channel;
        public RabbitMQProducerService(IConfiguration configuration)
        {
            factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Hostname"],
                Port = int.Parse(configuration["RabbitMQ:Port"]),
                UserName = configuration["RabbitMQ:Username"],
                Password = configuration["RabbitMQ:Password"],

            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            this.configuration = configuration;

        }
        public async Task ProduceMessage(string topic, string message)
        {
            var exchangeName = configuration["RabbitMQ:Exchange"];
            var queueName = configuration["RabbitMQ:Queue"];

            //channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

            channel.QueueDeclare(topic, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.BasicPublish("", $"{topic}", null, Encoding.UTF8.GetBytes(message));
        }
    }
}
