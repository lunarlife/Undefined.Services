namespace Undefined.Services.Application.Services.Lookup.Runtime.Dependencies;

internal interface IServiceDependenciesResolverBuilder
{
    public bool TryGetCachedOrBuildResolver(ServiceId serviceId, Type implementationType,
        out Action<CoreScope, IService>? resolverAction);
    public bool TryBuildResolverNoCache(ServiceId serviceId, Type implementationType,
        out Action<CoreScope, IService>? resolverAction);
}