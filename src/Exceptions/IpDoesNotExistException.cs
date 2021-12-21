namespace IpLookupProxy.Api.Exceptions;

public class IpDoesNotExistException : Exception
{
    public IpDoesNotExistException(string ipAddress)
        : base($"The IP address '{ipAddress}' does not exist")
    {
    }
}
