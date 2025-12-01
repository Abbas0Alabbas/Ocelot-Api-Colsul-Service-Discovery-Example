#nullable disable

namespace Consul_Shared.Models;

public class ServiceRegistrationInfo
{
    public string Id { get; set; }
    public string ServiceName { get; set; }
    public string Address { get; set; }

    public string HealthCheckUrl => $"http://{Address}:{Port}/health";

    public int Port { get; set; }
    public string[] Tags { get; set; }
}
