namespace IpLookupProxy.Api.Models;

internal class ClientsConfiguration
{
    public IReadOnlyDictionary<string, ClientConfigInfo> ClientConfigurations { get; }

    public ClientsConfiguration(IEnumerable<ClientConfigInfo> clientConfigurations)
    {
        ClientConfigurations = clientConfigurations.ToDictionary(x => x.Name);
    }

    public IEnumerable<string> GetRegisteredClients()
    {
        return ClientConfigurations.Keys;
    }

    public IEnumerable<string> GetEnabledClients()
    {
        return ClientConfigurations.Values.Where(x => x.Enabled).Select(x => x.Name);
    }

    public bool ClientExists(string clientName)
    {
        return ClientConfigurations.ContainsKey(clientName);
    }

    public bool ClientEnabled(string clientName)
    {
        if (!ClientExists(clientName))
            throw new KeyNotFoundException($"The client '{clientName}' was not found.");

        return ClientConfigurations[clientName].Enabled;
    }

    public void ToggleClientEnabled(string clientName, bool enable = true)
    {
        if (!ClientExists(clientName))
            throw new KeyNotFoundException($"The client '{clientName}' was not found.");

        ClientConfigurations[clientName].Enabled = enable;
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
