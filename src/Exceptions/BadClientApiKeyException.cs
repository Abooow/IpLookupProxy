namespace IpLookupProxy.Api.Exceptions;

public class BadClientApiKeyException : Exception
{
    public BadClientApiKeyException(string handlerName, string? clientName)
        : this(handlerName, clientName, null!)
    {
    }

    public BadClientApiKeyException(string handlerName, string? clientName, string message)
        : base($"Invalid API key for client '{handlerName}' ({clientName ?? "Unnamed"}). {message}")
    {
    }
}
