using Undefined.Services.Application.Services.Lookup;
using Undefined.Services.Application.Services.Lookup.Runtime.Factory;

namespace Undefined.Services.Application.Services;

internal class CompileServiceProviderEngine : ServiceProviderEngine
{
    private readonly ServiceDependenciesResolverBuilder _dependenciesResolverBuilder;

    public CompileServiceProviderEngine(ServiceProvider serviceProvider)
    {
        _dependenciesResolverBuilder = serviceProvider.DependenciesResolverBuilder;
    }

    public override Func<CoreScope, IService> RealizeService(ServiceCallSite callSite)
    {
        var builder = ServiceRuntimeResolverBuilder.Instance;
        if (callSite is TypeCallSite typeCallSite)
            return builder.GetCachedOrCreateFactoryFunction(typeCallSite.ServiceType,
                typeCallSite.ImplementationType,
                InjectService(callSite.Cache.ServiceId, typeCallSite.ImplementationType));
        Func<CoreScope, IService> factory = callSite switch
        {
            ValueCallSite valueCallSite => _ => valueCallSite.Value,
            FactoryCallSite factoryCallSite  => scope => factoryCallSite.ValueCached ??= factoryCallSite.Factory(scope),
            _ => throw new ArgumentException($"Unknown service call site: {callSite}.")
        };

        return builder.GetCachedOrCreateFactoryFunction(callSite,
            factory, type => InjectService(callSite.Cache.ServiceId, type));
    }

    private Action<CoreScope, IService> InjectService(ServiceId serviceId, Type implementationType) =>
        _dependenciesResolverBuilder.GetCachedOrCreateResolver(serviceId, implementationType);
}