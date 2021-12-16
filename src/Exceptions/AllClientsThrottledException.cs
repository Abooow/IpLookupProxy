namespace IpLookupProxy.Api.Exceptions;

public class AllClientsThrottledException : Exception
{
    public AllClientsThrottledException()
        : base("All the clients has throttled.")
    {
    }
}
