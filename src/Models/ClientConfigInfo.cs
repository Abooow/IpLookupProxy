using IpLookupProxy.Api.Exceptions;

namespace IpLookupProxy.Api.Models;

internal class ClientConfigInfo
{
    public string Name { get; set; } = default!;
    public string? Site { get; set; }
    public bool Enabled { get; set; } = true;
    public string? ApiKey { get; set; }
    public RateLimitRuleConfiguration[] RateLimitingRules { get; set; } = default!;

    public static void EnsureClientConfigsIsValid(ClientConfigInfo[] clientConfigs)
    {
        List<string> errorMessages = new();

        for (int i = 0; i < clientConfigs.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(clientConfigs[i].Name))
                errorMessages.Add($"The name field for ClientsConfig index: {i} can not be null or empty.");

            if (clientConfigs[i].RateLimitingRules is null || !clientConfigs[i].RateLimitingRules!.Any())
                errorMessages.Add($"At least 1 rate limiting rule must be set for ClientsConfig index: {i}. ({clientConfigs[i].Name})");
        }

        if (errorMessages.Any())
            throw new BadClientConfigurationsException(errorMessages);
    }
}

