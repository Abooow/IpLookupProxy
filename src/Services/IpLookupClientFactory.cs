using System.Collections.ObjectModel;
using System.Reflection;
using IpLookupProxy.Api.Options;
using IpLookupProxy.Api.Services.IpLookupClients;
using IpLookupProxy.Api.Services.LoadBalancers;

namespace IpLookupProxy.Api.Services;

internal class IpLookupClientFactory
{
    public static IEnumerable<string> RegisteredHandlers => ipLookupClientFactories.Keys;
    private static readonly ReadOnlyDictionary<string, Func<IServiceProvider, ClientConfigInfo, IIpLookupClient>> ipLookupClientFactories;

    private readonly IServiceProvider _serviceProvider;
    private readonly IClientLoadBalancer _clientLoadBalancer;

    static IpLookupClientFactory()
    {
        var ipLookupClientTypeFactoryPairs = typeof(Program).Assembly
            .GetTypes()
            .Where(x => x.IsAssignableTo(typeof(IIpLookupClient)) && x.IsClass)
            .Select(x => (Type: x, Factory: CreateIpLookupClientFactory(x)));

        var ipLookupClientFactoriesDic = new Dictionary<string, Func<IServiceProvider, ClientConfigInfo, IIpLookupClient>>();
        foreach (var typeFactoryPair in ipLookupClientTypeFactoryPairs)
        {
            foreach (var handlerAttribute in typeFactoryPair.Type.GetCustomAttributes<IpLookupClientHandlerAttribute>())
            {
                if (!ipLookupClientFactoriesDic.TryAdd(handlerAttribute.HandlerName, typeFactoryPair.Factory))
                    throw new Exception($"A handler with name '{handlerAttribute.HandlerName}' has already been registered.");
            }
        }

        ipLookupClientFactories = new ReadOnlyDictionary<string, Func<IServiceProvider, ClientConfigInfo, IIpLookupClient>>(ipLookupClientFactoriesDic);
    }

    private static Func<IServiceProvider, ClientConfigInfo, IIpLookupClient> CreateIpLookupClientFactory(Type clientType)
    {
        return (IServiceProvider serviceProvider, ClientConfigInfo clientConfigInfo) => (IIpLookupClient)ActivatorUtilities.CreateInstance(serviceProvider, clientType, clientConfigInfo);
    }

    public IpLookupClientFactory(IServiceProvider serviceProvider, IClientLoadBalancer clientLoadBalancer)
    {
        _serviceProvider = serviceProvider;
        _clientLoadBalancer = clientLoadBalancer;
    }

    public (IIpLookupClient Client, ClientConfigInfo configInfo) GetIpLookupClient()
    {
        var clientConfigInfo = _clientLoadBalancer.GetClientConfig();

        if (!ipLookupClientFactories.TryGetValue(clientConfigInfo.Handler, out var ipLookupFactory))
            throw new Exception($"'{clientConfigInfo.Handler}' is not a registered client.");

        return (ipLookupFactory.Invoke(_serviceProvider, clientConfigInfo), clientConfigInfo);
    }
}
