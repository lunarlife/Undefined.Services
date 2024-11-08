namespace Undefined.Services.Application.Services.Lookup;

internal abstract class ServiceCallSite
{
    private readonly bool _isDisposable;

    public Type ServiceType => Cache.ServiceId.ServiceType;
    public abstract Type? ImplementationType { get; }
    public CallSiteCache Cache { get; }
    public IService? ValueCached { get; set; }

    public bool IsDisposable => _isDisposable || IsDisposableType(ImplementationType);
    public bool HasImplementationType => ImplementationType is not null;
    public bool HasValueCached => ValueCached is not null;
    protected ServiceCallSite(CallSiteCache cache)
    {
        Cache = cache;
        _isDisposable = Cache.CallSiteCachePlace is CallSiteCachePlace.Dispose || IsDisposableType(ServiceType);
    }

    private static bool IsDisposableType(Type? type) =>
        type is not null &&
        (typeof(IDisposable).IsAssignableFrom(type) ||
         typeof(IAsyncDisposable).IsAssignableFrom(type));
}