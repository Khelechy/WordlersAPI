using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using WordlersAPI.Interfaces;
using WordlersAPI.Models.Constants;
using WordlersAPI.Models.Core;
using WordlersAPI.Models.DataModel;

namespace WordlersAPI.Services
{
    public class RabbitMQConsumerService : BackgroundService
    {
        private readonly ILogger<RabbitMQConsumerService> logger;
        private readonly IConfiguration configuration;
        private readonly IServiceProvider _serviceProvider;
        ConnectionFactory factory;
        IConnection connection;
        IModel channel;
        public RabbitMQConsumerService(ILogger<RabbitMQConsumerService> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.configuration = configuration;
            _serviceProvider = serviceProvider;
            factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Hostname"],
                Port = int.Parse(configuration["RabbitMQ:Port"]),
                UserName = configuration["RabbitMQ:Username"],
                Password = configuration["RabbitMQ:Password"],

            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ConsumeMessages(stoppingToken);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("RabbitMQ Consumer service is starting");
            return base.StartAsync(cancellationToken);
        }

        private async Task ConsumeMessages(CancellationToken cancellationToken)
        {
            try
            {
                await ConsumeStartGameTopic(Constant.StartGameRoundTopic);
                await ConsumeStopGameTopic(Constant.StopGameRoundTopic);
                await ConsumeUserGamePoint(Constant.StoreUserGamePoint);    


            }
            catch (Exception ex)
            {
                logger.LogError($"Rabbit MQ Consumer Error : {ex.Message}");
            }

        }

        private async Task ConsumeStartGameTopic(string topic)
        {

            channel.QueueDeclare(topic, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    logger.LogInformation($"Receieved message on start game topic ");

                    if (String.IsNullOrEmpty(message))
                    {
                        logger.LogInformation("Received empty message body");
                        return;
                    }

                    var response = JsonConvert.DeserializeObject<GameBrokerModel>(message);
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var gameService = scope.ServiceProvider.GetService<IGameService>();
                        var timeInMilliseconds = response.RoundDuration;
                        await gameService.TimeGameRound(response.GameId, timeInMilliseconds);
                    }

                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error from consuming start game topic : {ex.Message} ");
                }
            };
            channel.BasicConsume(topic, autoAck: false, consumer);
            logger.LogInformation($".. Listening for messages on {topic} topic ");

        }

        private async Task ConsumeStopGameTopic(string topic)
        {

            channel.QueueDeclare(topic, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    logger.LogInformation($"Receieved message on stop game topic ");

                    if (String.IsNullOrEmpty(message))
                    {
                        logger.LogInformation("Received empty message body");
                        return;
                    }

                    var response = JsonConvert.DeserializeObject<GameBrokerModel>(message);
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var gameService = scope.ServiceProvider.GetService<IGameService>();
                        await gameService.StopGameRound(response.GameId);
                    }

                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error from consuming stop game topic : {ex.Message} ");
                }
            };
            channel.BasicConsume(topic, autoAck: false, consumer);
            logger.LogInformation($".. Listening for messages on {topic} topic ");

        }


        //User Game Point Consumers
        private async Task ConsumeUserGamePoint(string topic)
        {

            channel.QueueDeclare(topic, durable: true, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    logger.LogInformation($"Receieved message on store user game point topic ");

                    if (String.IsNullOrEmpty(message))
                    {
                        logger.LogInformation("Received empty message body");
                        return;
                    }

                    var response = JsonConvert.DeserializeObject<UserGamePoint>(message);
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var userGamePointService = scope.ServiceProvider.GetService<IUserGamePointService>();
                        await userGamePointService.AddGamePoint(response);
                    }

                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error from consuming store user game point topic : {ex.Message} ");
                }
            };
            channel.BasicConsume(topic, autoAck: false, consumer);
            logger.LogInformation($".. Listening for messages on {topic} topic ");

        }
    }
}
