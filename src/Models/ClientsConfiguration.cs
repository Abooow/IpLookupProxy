namespace IpLookupProxy.Api.Models;

internal class ClientsConfiguration
{
    public IReadOnlyDictionary<string, ClientConfigInfo> ClientConfigurations { get; }

    public ClientsConfiguration(IEnumerable<KeyValuePair<string, ClientConfigInfo>> clientConfigurations)
    {
        ClientConfigurations = new Dictionary<string, ClientConfigInfo>(clientConfigurations);
    }

    public bool ClientExists(string clientName)
    {
        return ClientConfigurations.ContainsKey(clientName);
    }

    public string? GetApiKey(string clientName)
    {
        if (!ClientExists(clientName))
            throw new KeyNotFoundException($"The client '{clientName}' was not found.");

        return ClientConfigurations[clientName].ApiKey;
    }

    public ClientConfigInfo GetClientConfigInfo(string clientName)
    {
        if (!ClientExists(clientName))
            throw new KeyNotFoundException($"The client '{clientName}' was not found.");

        return ClientConfigurations[clientName];
    }
}
