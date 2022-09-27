using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using WordlersAPI.Data;
using WordlersAPI.Interfaces;
using WordlersAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<GameDbContext>(option => option.UseNpgsql
               (builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IWordEngineService, WordEngineService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IRabbitMQService, RabbitMQProducerService>();
builder.Services.AddHostedService<RabbitMQConsumerService>();

builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();

var multiplexer = ConnectionMultiplexer.Connect("localhost");
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["ConnectionStrings:RedisConnection"];
    options.InstanceName = "wordlers";
    options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
    {
        Password = builder.Configuration["RedisConnection:Host"],
        EndPoints =  {
                       { builder.Configuration["RedisConnection:Host"], int.Parse(builder.Configuration["RedisConnection:Port"])}
                    }
    };
});

builder.Services.AddSingleton<ICacheService, DistributedCacheService>();    



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
