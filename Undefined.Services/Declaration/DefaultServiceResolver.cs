using Undefined.Services.Scopes;

namespace Undefined.Services.Declaration;

internal class DefaultServiceResolver : IServiceResolver
{
    public Type ServiceType { get; }
    public Type ImplementationType { get; }
    public IApplicationProvider Provider { get; }
    public bool IsStatic { get; }
    public Scope Scope { get; }


    public DefaultServiceResolver(Scope scope, Type serviceType, Type implementationType,
        IApplicationProvider provider, bool isStatic)
    {
        Provider = provider;
        IsStatic = isStatic;
        Scope = scope;
        ServiceType = serviceType;
        ImplementationType = implementationType;
    }

    public IService Resolve() => Scope.Resolve(ServiceType);
}