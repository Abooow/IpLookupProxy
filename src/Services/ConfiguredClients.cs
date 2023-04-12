using System.Collections.ObjectModel;
using IpLookupProxy.Api.Exceptions;
using IpLookupProxy.Api.Options;

namespace IpLookupProxy.Api.Services;

internal class ConfiguredClients
{
    public IReadOnlyList<ClientConfigInfo> ClientConfigurations { get; }
    public ClientsRateLimiter ClientsRateLimiter { get; }

    public ConfiguredClients(IEnumerable<ClientConfigInfo> clientConfigurations, ILogger<ConfiguredClients> logger)
    {
        var validClientConfigs = GetOnlyValidClientConfigs(clientConfigurations, logger).ToList();

        if (!validClientConfigs.Any())
            throw new BadClientConfigurationsException("No valid client configurations has been registered.");

        ClientConfigurations = new ReadOnlyCollection<ClientConfigInfo>(validClientConfigs);
        ClientsRateLimiter = new ClientsRateLimiter(validClientConfigs);
    }


    private static IEnumerable<ClientConfigInfo> GetOnlyValidClientConfigs(IEnumerable<ClientConfigInfo> clientConfigs, ILogger<ConfiguredClients> logger)
    {
        int i = -1;
        foreach (var clientConfig in clientConfigs)
        {
            i++;

            if (string.IsNullOrWhiteSpace(clientConfig.Handler))
            {
                logger.LogWarning("The Handler field for ClientsConfig index: {Index} can not be null or empty, this client config will be ignored.", i);
                continue;
            }

            if (clientConfig.RateLimitingRules is null || !clientConfig.RateLimitingRules!.Any())
            {
                logger.LogWarning("At least 1 rate limiting rule must be set for ClientsConfig index: {Index} ({Handler}), this client config will be ignored.", i, clientConfig.Handler);
                continue;
            }

            if (!IpLookupClientFactory.RegisteredHandlers.Contains(clientConfig.Handler))
            {
                logger.LogWarning("No handler has been registered for {Handler}, index: {Index}, this client config will be ignored.", clientConfig.Handler, i);
                continue;
            }

            yield return clientConfig;
        }
    }

    public IEnumerable<(ClientConfigInfo Config, int Index)> GetEnabledClients()
    {
        return ClientConfigurations.Select((x, i) => (Config: x, Index: i)).Where(x => x.Config.Enabled);
    }
}
