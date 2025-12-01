using System.Security.Cryptography;
using CatalogService.Services.Consule;
using Consul;
using Consul_Shared;
using Consul_Shared.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
});

// Configure Consul settings from appsettings.json
builder.Services.Configure<ConsulConfig>(builder.Configuration.GetSection("Consul"));

// Register Consul service registration
builder.Services.AddSingleton<IHostedService, ConsulHostedService>();

builder.Services.RegisterConsule();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Health check endpoint
app.MapHealthChecks("/health");

// Sample endpoints
app.MapGet(
    "/api/product/{id}",
    async (string id, IConsulClient consulClient) =>
    {
        return Results.Ok(
            new
            {
                Id = id,
                Name = $"Product {id}",
                Price = 100 - RandomNumberGenerator.GetInt32(1, 100),
            }
        );
    }
);

app.MapControllers();

app.Run();
