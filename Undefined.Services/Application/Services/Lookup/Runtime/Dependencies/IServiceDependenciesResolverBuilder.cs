namespace Undefined.Services.Application.Services.Lookup.Runtime.Dependencies;

internal interface IServiceDependenciesResolverBuilder
{
    public Action<CoreScope, IService> GetCachedOrCreateResolver(ServiceId serviceId, Type implementationType);
    public Action<CoreScope, IService> CreateResolverNoCache(ServiceId serviceId, Type implementationType);
}