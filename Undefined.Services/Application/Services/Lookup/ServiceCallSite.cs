namespace Undefined.Services.Application.Services.Lookup;

internal abstract class ServiceCallSite
{
    public Type ServiceType => Cache.ServiceId.ServiceType;
    public abstract Type? ImplementationType { get; }
    
    public CallSiteCache Cache { get; }

    protected ServiceCallSite(CallSiteCache cache)
    {
        Cache = cache;
    }
}