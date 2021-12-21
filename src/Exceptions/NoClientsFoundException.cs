namespace IpLookupProxy.Api.Exceptions;

public class NoClientsFoundException : Exception
{
    public NoClientsFoundException()
    {
    }

    public NoClientsFoundException(string message)
        : base(message)
    {
    }
}
