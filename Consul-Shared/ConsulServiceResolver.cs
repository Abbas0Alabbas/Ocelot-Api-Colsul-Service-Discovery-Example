namespace Consul_Shared;

using Consul;

public interface IConsulServiceResolver
{
    Task<Uri> ResolveServiceAsync(string serviceName);
}

public class ConsulServiceResolver : IConsulServiceResolver
{
    private readonly IConsulClient _consul;
    private readonly Random _random = new();

    public ConsulServiceResolver(IConsulClient consul)
    {
        _consul = consul;
    }

    public async Task<Uri> ResolveServiceAsync(string serviceName)
    {
        // Get healthy service instances (checks must be passing)
        var result = await _consul.Health.Service(serviceName, tag: null, passingOnly: true);

        var instances = result.Response;

        if (instances is null || instances.Length == 0)
        {
            throw new Exception($"No healthy instances available for service: {serviceName}");
        }

        // Random-load-balance
        var selected = instances[_random.Next(instances.Length)];

        var address = selected.Service.Address;
        var port = selected.Service.Port;

        return new Uri($"http://{address}:{port}");
    }
}
