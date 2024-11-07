using Undefined.Services.Application.Services.Lookup;
using Undefined.Services.Application.Services.Lookup.Runtime.Injection;
using Undefined.Services.Application.Services.Scopes;

namespace Undefined.Services.Application.Services;

public class ServiceProvider : IServiceProvider
{
    private readonly CoreScope _rootScope;
    private readonly ServiceResolveMachine _resolveMachine;
    private readonly Dictionary<ServiceId, ServiceAccessor> _serviceAccessors = [];
    private readonly object _serviceCallSitesLock = new();
    private ServiceProviderEngine _engine;
    internal IServiceRuntimeInjector RuntimeInjector { get; }

    public ServiceProvider(ICollection<IServiceDescriptor> services, ICollection<ServiceInjectorBase> injectors)
    {
        RuntimeInjector = new ServiceRuntimeInjector(injectors);
        _resolveMachine = new ServiceResolveMachine(services);
        _engine = new CompileServiceProviderEngine();
        _rootScope = new CoreScope(this, true);
    }

    public IServiceScope CreateScope() => new CoreScope(this, false);

    public bool TryGetService(Type serviceType, out IService? service)
    {
        service = GetService(serviceType, _rootScope);
        return service is not null;
    }

    internal IService? GetService(Type serviceType, CoreScope scope)
    {
        var id = new ServiceId(serviceType);
        ServiceCallSite? callSite;
        lock (_serviceCallSitesLock)
        {
            if (!_serviceAccessors.TryGetValue(id, out var accessor))
                _serviceAccessors.Add(id, callSite = );
        }

        var cache = callSite.Cache;
        IService service;
        switch (cache.CallSiteCachePlace)
        {
            case CallSiteCachePlace.Root:
                service = _rootScope.Resolve(callSite, true);
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

    private ServiceAccessor CreateServiceAccessor(ServiceId serviceId)
    {
        lock (_serviceCallSitesLock)
        {
            var callSite = _resolveMachine.GetCachedOrCreateCallSite(serviceId);
            Func<CoreScope, IService> realizedService;
            if (callSite.Cache.CallSiteCachePlace == CallSiteCachePlace.Root)
            {
                var service = _rootScope.Resolve(callSite, true);
                realizedService = _ => service;
            }
            else
            {
                realizedService = _engine.RealizeService(callSite);
            }

            return new ServiceAccessor(callSite, realizedService);
        }
    }

    private class ServiceAccessor
    {
        public ServiceAccessor(ServiceCallSite callSite, Func<CoreScope, IService> realizedService)
        {
            CallSite = callSite;
            RealizedService = realizedService;
        }

        public ServiceCallSite CallSite { get; }
        public Func<CoreScope, IService> RealizedService { get; }
    }
}