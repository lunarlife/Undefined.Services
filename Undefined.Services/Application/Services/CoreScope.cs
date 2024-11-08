using System.Collections.Concurrent;
using Undefined.Services.Application.Services.Lookup;
using Undefined.Services.Application.Services.Scopes;
using Undefined.Services.Exceptions;

namespace Undefined.Services.Application.Services;

internal class CoreScope : IServiceScope, IServiceProvider
{
    private readonly ServiceProvider _rootProvider;
    private readonly bool _isRootScope;
    private readonly ConcurrentDictionary<ServiceId, IService> _resolvedServices = [];
    private readonly List<IService> _disposables = [];
    public IServiceProvider ServiceProvider => this;

    public CoreScope(ServiceProvider rootProvider, bool isRootScope)
    {
        _rootProvider = rootProvider;
        _isRootScope = isRootScope;
    }


    public IServiceScope CreateScope() => _rootProvider.CreateScope();

    public bool TryGetService(Type serviceType, out IService? service)
    {
        service = _rootProvider.GetService(serviceType, this);
        return service is not null;
    }


    public IService AddOrGetCachedService(ServiceCallSite callSite, Func<CoreScope, IService> realizeService) =>
        _resolvedServices.GetOrAdd(callSite.Cache.ServiceId,_ =>
        {
            var service = realizeService(this);
            if (callSite.IsDisposable)
                AddDisposable(service);
            return service;
        });

    public void AddDisposable(IService disposable)
    {
        _disposables.Add(disposable);
    }

    internal IService Resolve(ServiceCallSite callSite, bool cacheValue)
    {
        var cache = callSite.Cache;
        IService service;

        var isAlreadyResolved = false;
        if (cacheValue)
        {
            if (_resolvedServices.TryGetValue(cache.ServiceId, out service!))
                isAlreadyResolved = true;
            else
            {
                if (!_resolvedServices.TryAdd(cache.ServiceId, service = ResolveNotCachedService(callSite)))
                    throw new ServiceResolveException("SOMETHING UNEXPECTED HAPPENED.");
            }
        }
        else service = ResolveNotCachedService(callSite);

        if (service is IDisposable or IAsyncDisposable) _disposables.Add(service);

        if (!isAlreadyResolved)
            _rootProvider.DependenciesResolverBuilder.GetCachedOrCreateResolver(cache.ServiceId, service.GetType())(this, service);
        return service;
    }

    public void Dispose()
    {
        foreach (var service in _disposables)
        {
            if (service is IDisposable disposable)
            {
                disposable.Dispose();
            }
            else
                Task.Run(() => ((IAsyncDisposable)service).DisposeAsync().AsTask()).GetAwaiter().GetResult();
        }
    }
}