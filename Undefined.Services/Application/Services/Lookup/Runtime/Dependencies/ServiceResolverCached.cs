using System.Reflection;

namespace Undefined.Services.Application.Services.Lookup.Runtime.Dependencies;

internal class ServiceResolverCached
{
    public Type ImplementationType { get; }
    public MethodInfo? OriginalMethod { get; }
    public Action<CoreScope, IService>? Resolver { get; }

    public bool HasResolver => Resolver is not null;

    public ServiceResolverCached(Type implementationType, MethodInfo originalMethod,
        Action<CoreScope, IService> resolver)
    {
        ImplementationType = implementationType;
        OriginalMethod = originalMethod;
        Resolver = resolver;
    }

    public ServiceResolverCached(Type implementationType)
    {
        ImplementationType = implementationType;
    }
}