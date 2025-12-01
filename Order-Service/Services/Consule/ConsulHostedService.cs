// Consul Service Registration - Auto-register and deregister
using Consul_Shared;
using Consul_Shared.Models;
using Microsoft.Extensions.Options;

namespace OrderService.Services.Consule;

public class ConsulHostedService : IHostedService
{
    private readonly ConsulService _consulService;
    private readonly ILogger<ConsulHostedService> _logger;
    private readonly ConsulConfig _config;
    private string _registrationId;

    public ConsulHostedService(
        ConsulService consulService,
        ILogger<ConsulHostedService> logger,
        IOptions<ConsulConfig> config
    )
    {
        _consulService = consulService;
        _logger = logger;
        _config = config.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _registrationId = _config.ServiceId;

        await _consulService.DeregisterService(_registrationId, cancellationToken);
        await _consulService.RegisterService(
            new ServiceRegistrationInfo
            {
                Id = _registrationId,
                ServiceName = _config.ServiceName,
                Address = _config.ServiceAddress,
                Port = _config.ServicePort,
                Tags = _config.ServiceTags,
            },
            cancellationToken
        );
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _consulService.DeregisterService(_registrationId, cancellationToken);
    }
}
