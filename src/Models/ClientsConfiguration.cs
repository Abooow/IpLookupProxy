namespace IpLookupProxy.Api.Models;

internal class ClientsConfiguration
{
    public IReadOnlyDictionary<string, ClientConfigInfo> ClientConfigurations { get; }

    public ClientsConfiguration(IEnumerable<ClientConfigInfo> clientConfigurations)
    {
        ClientConfigurations = clientConfigurations.ToDictionary(x => x.Name);
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
