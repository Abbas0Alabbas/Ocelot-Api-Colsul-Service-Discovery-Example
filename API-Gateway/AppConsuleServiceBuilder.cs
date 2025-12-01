using Ocelot.Logging;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Consul.Interfaces;
using Consul;
using Ocelot.Configuration.Builder;

public class AppConsuleServiceBuilder : DefaultConsulServiceBuilder
{
    public AppConsuleServiceBuilder(
        IHttpContextAccessor contextAccessor,
        IConsulClientFactory clientFactory,
        IOcelotLoggerFactory loggerFactory
    )
        : base(contextAccessor, clientFactory, loggerFactory) { }

    // Use the agent service IP address as the downstream hostname
    protected override string GetDownstreamHost(ServiceEntry entry, Node node) =>
        entry.Service.Address;
}
