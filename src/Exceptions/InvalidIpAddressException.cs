namespace IpLookupProxy.Api.Exceptions;

public class InvalidIpAddressException : Exception
{
    public InvalidIpAddressException(string ipAddress)
        : base($"The IP address '{ipAddress}' is not a valid IPv4 or IPv6 address")
    {
    }
}
