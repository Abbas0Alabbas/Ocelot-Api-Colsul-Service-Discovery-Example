using Consul;
using Consul_Shared;
using Consul_Shared.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Consul_Shared;

public static class ServiceDIRegistration
{
    public static void RegisterConsule(this IServiceCollection services)
    {
        services.AddSingleton<ConsulService>();
        // Configure Consul settings from appsettings.json
        services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
        {
            var config = p.GetRequiredService<IOptions<ConsulConfig>>().Value;
            consulConfig.Address = new Uri(config.Address);
        }));
    }
}
