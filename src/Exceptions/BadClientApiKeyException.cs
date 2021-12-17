namespace IpLookupProxy.Api.Exceptions;

public class BadClientApiKeyException : Exception
{
    public BadClientApiKeyException(string clientName)
        : this(clientName, null!)
    {
    }

    public BadClientApiKeyException(string clientName, string message)
    : base($"Invalid API key for client '{clientName}'. {message}")
    {
    }
}
