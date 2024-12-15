using System.Text;
using access_service.Src.Data;
using access_service.Src.Helpers;
using access_service.Src.Middlewares;
using access_service.Src.Repositories;
using access_service.Src.Repositories.Interfaces;
using access_service.Src.Services;
using access_service.Src.Services.Interfaces;
using DotNetEnv;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Messages;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

var host = Env.GetString("DB_HOST");
var port = Env.GetString("DB_PORT");
var dbName = Env.GetString("DB_NAME");
var user = Env.GetString("DB_USER");
var password = Env.GetString("DB_PASSWORD");

var connectionString = $"Host={host};Port={port};Database={dbName};Username={user};Password={password}";

builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IBlackListService, BlackListService>();

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<Seed>();

var secret = Env.GetString("JWT_SECRET");

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            secret
        ))
    };
});

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost","/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.Send<CreateUserMessage>(config =>
            config.UseRoutingKeyFormatter(context => "create-user-queue")
        );

        cfg.Send<TokenToBlacklistMessage>(config =>
            {
                config.UseRoutingKeyFormatter(context => "token-blacklist-queue-1");
                config.UseRoutingKeyFormatter(context => "token-blacklist-queue-2");
            }
        );
    });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seed = scope.ServiceProvider.GetRequiredService<Seed>();
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    seed.SeedData(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<BlackListMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();


