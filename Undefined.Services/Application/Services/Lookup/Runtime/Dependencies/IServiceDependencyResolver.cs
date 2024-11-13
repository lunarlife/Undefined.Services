using System.Reflection;
using Undefined.Services.Application.Services.Scopes;

namespace Undefined.Services.Application.Services.Lookup.Runtime.Dependencies;


public readonly struct ServiceDependencySuitableContext
{
    public ParameterInfo Parameter { get; }

    public ServiceDependencySuitableContext(ParameterInfo parameter)
    {
        
        Parameter = parameter;
    }

    
}

public readonly struct ServiceDependencyResolveContext
{
    public IServiceScope Scope { get; }
    public IService Service { get; }

    public ServiceDependencyResolveContext(IServiceScope scope, IService service)
    {
        Scope = scope;
        Service = service;
    }
}

public abstract class ServiceDependencyResolver
{
    public abstract Func<ServiceDependencySuitableContext, bool> IsSuitableFunc { get; }
}

public class ServicesServiceDependencyResolver : ServiceDependencyResolver
{
    public override Func<ServiceDependencySuitableContext, bool> IsSuitableFunc => context =>
    {
        context.Parameter.
    };
}

public interface IServiceDependencyResolverBuilder
{
    public IServiceDependencyResolverBuilder Type(Type type);
    public IServiceDependencyResolverBuilder TypesExtendsFrom(Type type);

    public void Build<T>(Func<ServiceDependencyResolveContext, T> resolveFunc);
}