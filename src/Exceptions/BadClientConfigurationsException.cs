namespace IpLookupProxy.Api.Exceptions;

public class BadClientConfigurationsException : Exception
{
    public BadClientConfigurationsException(string message)
    : base(message)
    {
    }

    public BadClientConfigurationsException(IEnumerable<string> messages)
        : base(string.Join('\n', messages))
    {
    }
}
