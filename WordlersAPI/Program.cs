using Microsoft.EntityFrameworkCore;
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
