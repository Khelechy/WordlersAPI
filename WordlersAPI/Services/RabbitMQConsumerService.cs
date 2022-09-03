using RabbitMQ.Client;

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

            
        }
    }
}
