namespace IpLookupProxy.Api.Exceptions;

public class BadClientConfigurationsException : Exception
{
    public BadClientConfigurationsException(string message)
    : base(message)
    {
    }
}
