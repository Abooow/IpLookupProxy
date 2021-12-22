namespace IpLookupProxy.Api.Exceptions;

public class ClientErrorException : Exception
{
    public ClientErrorException(string clientName)
    : this(clientName, null!)
    {
    }

    public ClientErrorException(string clientName, string message)
        : base($"An unexpected error occurred for client '{clientName}'. {message}")
    {
    }
}
