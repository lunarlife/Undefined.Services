namespace Undefined.Services.Application.Services.Lookup.Runtime.Injection;

internal interface IServiceRuntimeInjector
{
    public Action<CoreScope, IService> GetCachedOrCreateInjector(ServiceId serviceId, Type implementationType);
    public Action<CoreScope, IService> CreateResolverNoCache(ServiceId serviceId, Type implementationType);
}