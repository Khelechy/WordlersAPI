using MessagePack;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using WordlersAPI.Data;
using WordlersAPI.Hubs;
using WordlersAPI.Interfaces;
using WordlersAPI.Mapper;
using WordlersAPI.Models.Core;
using WordlersAPI.Models.Helper;
using WordlersAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAutoMapper(typeof(WordlerMapping));
builder.Services.AddSignalR()
    .AddMessagePackProtocol();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//CORS

builder.Services.AddCors(feature =>
    feature.AddPolicy(
        "CorsPolicy",
        apiPolicy => apiPolicy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(host => true)
            .AllowCredentials()
));

builder.Services.AddDbContext<GameDbContext>(option => option.UseNpgsql
               (builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 5;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<GameDbContext>()
                    .AddDefaultTokenProviders();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddScoped<IRoomService, RoomService>();    
builder.Services.AddScoped<IGameHubService, GameHubService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IWordEngineService, WordEngineService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IUserGamePointService, UserGamePointService>();  
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

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
using (var scope = app.Services.CreateScope())
{
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await ContextSeed.SeedRolesAsync(userManager, roleManager);
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<GameHub>("/gameHub");

app.Run();
