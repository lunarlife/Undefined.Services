using System.Collections.Concurrent;
using System.Reflection.Emit;

namespace Undefined.Services.Application.Services.Lookup.Runtime.Factory;

internal class ServiceRuntimeFactoryGenerator : IServiceRuntimeFactoryGenerator
{
    public static IServiceRuntimeFactoryGenerator Instance { get; } = new ServiceRuntimeFactoryGenerator();
    
    private readonly ConcurrentDictionary<Type, RuntimeServiceFactory> _cachedFactories = [];
    private readonly object _lock = new();
    
    private ServiceRuntimeFactoryGenerator()
    {
        
    }
    public Func<IServiceProvider, IService> GetCachedOrCreateFactoryFunction(Type serviceType, Type implementationType, Action<CoreScope, IService>? injectFunc)
    {
        var factory = _cachedFactories.GetOrAdd(serviceType, type => CreateFactory(type, implementationType, injectFunc));
        return factory.Resolver;
    }

    public Func<IServiceProvider, IService> CreateFactoryFunctionNoCache(Type serviceType, Type implementationType, Action<CoreScope, IService>? injectFunc) => throw new NotImplementedException();

    public Func<IServiceProvider, IService> CreateFactoryFunctionCached(Type serviceType, Type implementationType, Action<CoreScope, IService>? injectFunc) => throw new NotImplementedException();

    private RuntimeServiceFactory CreateFactory(Type serviceType, Type implementationType, Action<CoreScope, IService>? injectFunc)
    {
        var method = new DynamicMethod($"service_factory_{serviceType.Name}", typeof(IService), [typeof(IServiceProvider)]);
        var generator = method.GetILGenerator();
        
        //instantiate a implementation type and flush all services and filters
        return new RuntimeServiceFactory
        {
            Resolver = (Func<IServiceProvider, IService>)method.CreateDelegate(typeof(Func<IServiceProvider, IService>))
        };
    }
    private class RuntimeServiceFactory
    {
        public Func<IServiceProvider, IService> Resolver;
    }
}