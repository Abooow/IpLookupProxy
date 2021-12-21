namespace IpLookupProxy.Api.Exceptions;

public class NoClientsException : Exception
{
    public NoClientsException()
    {
    }

    public NoClientsException(string message)
        : base(message)
    {
    }
}
