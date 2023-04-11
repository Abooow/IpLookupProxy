using System.Collections.ObjectModel;
using IpLookupProxy.Api.Options;

namespace IpLookupProxy.Api.Services;

internal class ConfiguredClients
{
    public IReadOnlyList<ClientConfigInfo> ClientConfigurations { get; }
    public ClientsRateLimiter ClientsRateLimiter { get; }

    public ConfiguredClients(IEnumerable<ClientConfigInfo> clientConfigurations)
    {
        ClientConfigurations = new ReadOnlyCollection<ClientConfigInfo>(clientConfigurations.ToList());
        ClientsRateLimiter = new ClientsRateLimiter(clientConfigurations);
    }

    public IEnumerable<ClientConfigInfo> GetEnabledClients()
    {
        return ClientConfigurations.Where(x => x.Enabled);
    }
}
