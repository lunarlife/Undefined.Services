namespace Undefined.Services.Application.Services.Lookup;

public readonly struct CallSiteCache
{
    public ServiceId ServiceId { get; }
    public CallSiteCachePlace CallSiteCachePlace { get; }

    public CallSiteCache(Type serviceType)
    {
        ServiceId = new ServiceId(serviceType);
    }
    public CallSiteCache(ServiceLifetime lifetime, ServiceId serviceId)
    {
        CallSiteCachePlace = lifetime switch
        {
            ServiceLifetime.Singleton => CallSiteCachePlace.Root,
            ServiceLifetime.Scoped => CallSiteCachePlace.Scope,
            ServiceLifetime.Transient => CallSiteCachePlace.Dispose,
            _ => CallSiteCachePlace.Nowhere
        };
        ServiceId = serviceId;
    }

    public static CallSiteCache Nowhere(Type serviceType) => new(serviceType);
}