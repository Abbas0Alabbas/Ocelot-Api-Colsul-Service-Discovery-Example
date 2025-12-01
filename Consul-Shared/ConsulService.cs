#nullable disable

// Consul Service Registration - Auto-register and deregister
using Consul;
using Consul_Shared.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Consul_Shared;

public class ConsulService
{
    private readonly IConsulClient _consulClient;
    private readonly ILogger _logger;
    private readonly ConsulConfig _config;

    public ConsulService(
        IConsulClient consulClient,
        ILogger<ConsulService> logger,
        IOptions<ConsulConfig> config
    )
    {
        _consulClient = consulClient;
        _logger = logger;
        _config = config.Value;
    }

    public async Task RegisterService(
        ServiceRegistrationInfo serviceInfo,
        CancellationToken cancellationToken
    )
    {
        var registration = new AgentServiceRegistration
        {
            ID = serviceInfo.Id,
            Name = serviceInfo.ServiceName,
            Address = serviceInfo.Address,
            Port = serviceInfo.Port,
            Tags = serviceInfo.Tags,

            Check = new AgentServiceCheck
            {
                //TTL = TimeSpan.FromSeconds(_config.Ttl),
                HTTP = serviceInfo.HealthCheckUrl,
                Interval = TimeSpan.FromSeconds(_config.CheckInterval),
                Timeout = TimeSpan.FromSeconds(5),
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(
                    _config.DeregisterCriticalServiceAfterMinutes
                ),
            },
        };

        try
        {
            await _consulClient.Agent.ServiceDeregister(serviceInfo.Id, cancellationToken);
            await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
            _logger.LogInformation("Service registered with Consul: {ServiceId}", serviceInfo.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Failed to register service with Consul {_config.CheckInterval} seconds"
            );
        }
    }

    public async Task DeregisterService(string serviceId, CancellationToken cancellationToken)
    {
        try
        {
            await _consulClient.Agent.ServiceDeregister(serviceId, cancellationToken);
            _logger.LogInformation("Service deregistered from Consul: {ServiceId}", serviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deregister service from Consul");
        }
    }

    public async Task<Uri> ResolveServiceAsync(string serviceName)
    {
        // Get healthy service instances (checks must be passing)
        var result = await _consulClient.Health.Service(serviceName, tag: null, passingOnly: true);

        var instances = result.Response;

        if (instances is null || instances.Length == 0)
        {
            throw new Exception($"No healthy instances available for service: {serviceName}");
        }

        // Random-load-balance
        var selected = instances[Random.Shared.Next(instances.Length)];

        var address = selected.Service.Address;
        var port = selected.Service.Port;

        return new Uri($"http://{address}:{port}");
    }
}
