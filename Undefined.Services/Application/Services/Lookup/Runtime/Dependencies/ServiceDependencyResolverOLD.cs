using System.Reflection;

namespace Undefined.Services.Application.Services.Lookup.Runtime.Dependencies;

public delegate TDependency DependencyResolver<TDependency>(IServiceProvider provider, IService service);

public abstract class ServiceDependencyResolverBaseOLD
{
    public abstract Type? BaseDependencyType { get; }

    internal ServiceDependencyResolverBaseOLD()
    {
    }
}

public abstract class ServiceDependencyResolverOLD : ServiceDependencyResolverBaseOLD
{
    internal const string GET_DEPENDENCY_RESOLVER_METHOD_NAME = nameof(GetDependencyResolver);

    public override Type? BaseDependencyType => null;

    public abstract DependencyResolver<TDependency>? GetDependencyResolver<TDependency>(IServiceProvider rootProvider,
        ParameterInfo parameter);
}

public abstract class ServiceDependencyResolver<TBase> : ServiceDependencyResolverBaseOLD
{
    public sealed override Type BaseDependencyType { get; } = typeof(TBase);

    public abstract DependencyResolver<TDependency>? GetDependencyResolver<TDependency>(IServiceProvider rootProvider,
        ParameterInfo parameter) where TDependency : TBase;
}