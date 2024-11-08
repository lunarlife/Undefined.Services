using System.Reflection;

namespace Undefined.Services.Application.Services.Lookup.Runtime.Dependencies;

internal class ServicesDependencyResolver : ServiceDependencyResolver<IService>
{
    public override DependencyResolver<TService>? GetDependencyResolver<TService>(IServiceProvider rootProvider,
        ParameterInfo parameter)
    {
        var func = ((ServiceProvider)rootProvider).GetRealizedService(typeof(TService));
        return (provider, _) => (TService)func((CoreScope)provider);
    }
}