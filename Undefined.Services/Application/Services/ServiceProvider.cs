using Undefined.Services.Application.Services.Lookup;
using Undefined.Services.Application.Services.Scopes;

namespace Undefined.Services.Application.Services;

internal class ServiceProvider : IServiceProvider
{
    private readonly ServiceResolveMachine _resolveMachine;
    private readonly Dictionary<ServiceId, ServiceAccessor> _serviceAccessors = [];
    private readonly object _serviceCallSitesLock = new();
    private readonly ServiceProviderEngine _engine;
    internal ServiceDependenciesResolverBuilder DependenciesResolverBuilder { get; }

    public CoreScope RootScope { get; }

    public ServiceProvider(ICollection<IServiceDescriptor> services, ICollection<ServiceDependencyResolver> injectors)
    {
        DependenciesResolverBuilder = new ServiceDependenciesResolverBuilder(injectors);
        _resolveMachine = new ServiceResolveMachine(services);
        RootScope = new CoreScope(this, true);
        _engine = new CompileServiceProviderEngine(this);
    }

    public IServiceScope CreateScope() => new CoreScope(this, false);

    public bool TryGetService(Type serviceType, out IService? service)
    {
        service = GetService(serviceType, RootScope);
        return service is not null;
    }

    internal IService? GetService(Type serviceType, CoreScope scope)
    {
        var id = new ServiceId(serviceType);
        lock (_serviceCallSitesLock)
        {
            if (!_serviceAccessors.TryGetValue(id, out var accessor))
            {
                var callSite = _resolveMachine.GetCachedOrCreateCallSite(id);
                _serviceAccessors.Add(id,
                    accessor = new ServiceAccessor(callSite, FirstRealizeService(_engine.RealizeService(callSite))));
            }
        }

        var cache = callSite.Cache;
        IService service;
        switch (cache.CallSiteCachePlace)
        {
            case CallSiteCachePlace.Root:
                service = RootScope.Resolve(callSite, true);
                break;
            case CallSiteCachePlace.Scope:
                service = scope.Resolve(callSite, true);
                break;
            case CallSiteCachePlace.Dispose:
            case CallSiteCachePlace.Nowhere:
                service = scope.Resolve(callSite, false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return service;
    }

    private Func<CoreScope, IService> FirstRealizeService(ServiceCallSite callSite,
        Func<CoreScope, IService> realizeServiceFunc)
    {
        return scope =>
        {
            var place = callSite.Cache.CallSiteCachePlace;
            Func<CoreScope, IService> realizedService;
            if (place is CallSiteCachePlace.Root)
            {
                var cached = RootScope.AddOrGetCachedService(callSite, realizeServiceFunc);
                realizedService = _ => cached;
            }
            else if (place == CallSiteCachePlace.Scope)
            {
                realizedService = sc => sc.AddOrGetCachedService(callSite, realizeServiceFunc);
            }
            else
            {
                realizedService = realizeServiceFunc;
            }

            _serviceAccessors[callSite.Cache.ServiceId].RealizedService = realizedService;
            return realizedService(scope);
        };
    }

    private ServiceAccessor CreateServiceAccessor(ServiceId serviceId)
    {
        lock (_serviceCallSitesLock)
        {
            var callSite = _resolveMachine.GetCachedOrCreateCallSite(serviceId);
            Func<CoreScope, IService> realizedService;

            realizedService = _engine.RealizeService(callSite);


            return new ServiceAccessor(callSite, realizedService);
        }
    }

    internal Func<CoreScope, IService> GetRealizedService(Type serviceType)
    {
        var id = new ServiceId(serviceType);
        if (!_serviceAccessors.TryGetValue(id, out var accessor))
            _serviceAccessors.Add(id, accessor = CreateServiceAccessor(id));
        return accessor.RealizedService;
    }

    private class ServiceAccessor
    {
        public ServiceAccessor(ServiceCallSite callSite, Func<CoreScope, IService> realizedService)
        {
            CallSite = callSite;
            RealizedService = realizedService;
        }

        public ServiceCallSite CallSite { get; }
        public Func<CoreScope, IService> RealizedService { get; set; }
    }
}