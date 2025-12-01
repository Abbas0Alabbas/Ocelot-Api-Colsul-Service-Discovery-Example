using System.Security.Cryptography;
using Consul;
using Consul_Shared;
using Consul_Shared.Models;
using Microsoft.Extensions.Options;
using OrderService.Models;
using OrderService.Services.Consule;

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

// Register Consul client
builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
    var config = p.GetRequiredService<IOptions<ConsulConfig>>().Value;
    consulConfig.Address = new Uri(config.Address);
}));

builder.Services.RegisterConsule();

// Register Consul service registration
builder.Services.AddSingleton<IHostedService, ConsulHostedService>();

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
app.MapPost(
    "/api/order",
    async (CreateOrderRequest request, ConsulService consulService) =>
    {
        var catalogService = await consulService.ResolveServiceAsync("CatalogService");
        var catalogServiceUrl = catalogService.AbsoluteUri.TrimEnd('/');
        using var httpClient = new HttpClient();
        var products = await Task.WhenAll(
            request.Items.Select(item =>
                httpClient.GetFromJsonAsync<Product>(
                    new Uri($"{catalogServiceUrl}/api/product/{item.ProductId}")
                )
            )
        );
        if (products.Any(p => p == null))
            return Results.NotFound("Some products not found");

        return Results.Ok(
            new { OrderId = Guid.NewGuid(), TotalPrice = products.Sum(p => p!.Price) }
        );
    }
);

app.MapControllers();
app.Run();
