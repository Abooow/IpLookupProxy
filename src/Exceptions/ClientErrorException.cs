namespace IpLookupProxy.Api.Exceptions;

public class ClientErrorException : Exception
{
    public ClientErrorException(string handlerName, string? clientName)
    : this(handlerName, clientName, null!)
    {
    }

    public ClientErrorException(string handlerName, string? clientName, string message)
        : base($"An unexpected error occurred for client '{handlerName}' ({clientName ?? "Unnamed"}). {message}")
    {
    }
}
