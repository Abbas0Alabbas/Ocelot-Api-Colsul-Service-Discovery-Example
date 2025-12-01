using Newtonsoft.Json;

namespace Consul_Shared.Models;

// Consul Configuration Model
public class ConsulConfig
{
    public string Address { get; set; } = "http://localhost:8500";
    public string ServiceName { get; set; } = "my-dotnet-service";
    public string ServiceId { get; set; } = $"my-dotnet-service-{Guid.NewGuid()}";
    public string ServiceAddress { get; set; } = "localhost";
    public int ServicePort { get; set; } = 5000;
    public string[] ServiceTags { get; set; } = { "api", "dotnet" };

    public int CheckInterval { get; set; } = 10;
    public int DeregisterCriticalServiceAfterMinutes { get; set; } = 1;
}
