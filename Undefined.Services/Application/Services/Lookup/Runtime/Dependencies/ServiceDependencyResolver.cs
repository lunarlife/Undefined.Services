using System.Reflection;

namespace Undefined.Services.Application.Services.Lookup.Runtime.Dependencies;

public delegate TDependency DependencyResolver<TDependency>(IServiceProvider provider,IService service);

public abstract class ServiceDependencyResolverBase
{
    public abstract Type? BaseDependencyType { get; }

    internal ServiceDependencyResolverBase()
    {
        
    }
}

public abstract class ServiceDependencyResolver : ServiceDependencyResolverBase
{
    internal const string GET_DEPENDENCY_RESOLVER_METHOD_NAME = nameof(GetDependencyResolver);

    public override Type? BaseDependencyType => null;

    public abstract DependencyResolver<TDependency>? GetDependencyResolver<TDependency>(IServiceProvider rootProvider, ParameterInfo parameter);
}

public abstract class ServiceDependencyResolver<TBase> : ServiceDependencyResolverBase
{
    public sealed override Type BaseDependencyType { get; } = typeof(TBase);
    public abstract DependencyResolver<TDependency>? GetDependencyResolver<TDependency>(IServiceProvider rootProvider, ParameterInfo parameter) where TDependency : TBase;
}