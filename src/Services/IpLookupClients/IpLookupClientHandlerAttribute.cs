namespace IpLookupProxy.Api.Services.IpLookupClients;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class IpLookupClientHandlerAttribute : Attribute
{
    public string HandlerName { get; }

    public IpLookupClientHandlerAttribute(string handlerName)
    {
        HandlerName = handlerName;
    }
}
