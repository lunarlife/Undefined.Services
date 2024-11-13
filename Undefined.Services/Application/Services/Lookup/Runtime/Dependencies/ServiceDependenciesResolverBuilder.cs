using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using Undefined.Services.Exceptions;

namespace Undefined.Services.Application.Services.Lookup.Runtime.Dependencies;

internal class ServiceDependenciesResolverBuilder : IServiceDependenciesResolverBuilder
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ServiceDependencyResolverOLD[] _injectors;

    private readonly ConcurrentDictionary<ServiceId, ServiceResolverCached> _cachedResolvers = [];

    internal ServiceDependenciesResolverBuilder(ServiceProvider serviceProvider,
        ICollection<ServiceDependencyResolverOLD> injectors)
    {
        _serviceProvider = serviceProvider;
        _injectors = new ServiceDependencyResolverOLD[injectors.Count];
        injectors.CopyTo(_injectors, 0);
        for (var i = 0; i < injectors.Count; i++)
        {
            var injector = _injectors[i];
            if (injector.BaseDependencyType is not { } bt) continue;
            if (bt.IsGenericType)
                throw new DependencyResolveException("Generic dependency resolvers are not supported.");
        }
    }


    public bool TryGetCachedOrBuildResolver(ServiceId serviceId, Type implementationType,
        out Action<CoreScope, IService>? resolverAction)
    {
        resolverAction = _cachedResolvers.GetOrAdd(serviceId, _ => BuildResolver(serviceId, implementationType))
            .Resolver;
        return resolverAction != null;
    }

    public bool TryBuildResolverNoCache(ServiceId serviceId, Type implementationType,
        out Action<CoreScope, IService>? resolverAction)
    {
        var resolver = BuildResolver(serviceId, implementationType);
        resolverAction = resolver.Resolver;
        return resolverAction is not null;
    }

    private ServiceResolverCached BuildResolver(ServiceId serviceId, Type implementationType)
    {
        if (!TryGetServiceResolverMethod(implementationType, out var serviceResolverMethod))
            return new ServiceResolverCached(implementationType);
        var parameters = serviceResolverMethod!.GetParameters();
        if (parameters.Length == 0)
        {
            var action = (Action)serviceResolverMethod.CreateDelegate(typeof(Action));
            return new ServiceResolverCached(implementationType, serviceResolverMethod, (_, _) => action());
        }

        var method = new DynamicMethod($"service_{serviceId}_dep_resolve", typeof(void),
            [typeof(CoreScope), typeof(IService)]);
        var generator = method.GetILGenerator();

        foreach (var parameter in parameters)
        {
            var parameterType = parameter.ParameterType;
        }

        for (var i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            foreach (var injector in _injectors)
            {
                ApplyInjector(injector, field, generator);
            }
        }

        generator.Emit(OpCodes.Ret);
        return (Action<CoreScope, IService>)method.CreateDelegate(typeof(Action<CoreScope, IService>));
    }

    private static bool TryGetServiceResolverMethod(Type implementationType, out MethodInfo? method)
    {
        var methods = implementationType.GetMethods(BindingFlags.Instance);
        foreach (var m in methods)
        {
            if (m.GetCustomAttribute<ResolverAttribute>() is null) continue;
            if (!m.IsPrivate)
                throw new InvalidOperationException(
                    $"Method {m.Name} has {nameof(ResolverAttribute)} but is not private.");
            method = m;
            return true;
        }

        method = null;
        return false;
    }

    private void ApplyInjector(ILGenerator generator, ServiceDependencyResolverOLD injector, ParameterInfo parameter)
    {
        if (injector.BaseDependencyType is { } baseInjectionType &&
            !baseInjectionType.IsAssignableFrom(parameter.ParameterType)) return;
        var injectorType = injector.GetType();
        if (GetResolveFunc(injectorType.IsGenericType, parameter, injector) is not { } resolveMethod) return;

        //Call injector resolve function    (Func<CoreScope, IService, TField>)
        generator.Emit(OpCodes.Ldarg_0);
        generator.Emit(OpCodes.Ldarg_1);
        generator.Emit(OpCodes.Call, resolveMethod);
        generator.Emit(OpCodes.Stfld, parameter);
    }

    private MethodInfo? GetResolveFunc(bool isGeneric, ParameterInfo parameter, ServiceDependencyResolverOLD injector) =>
        ((isGeneric ? _getResolveFunctionMethodGeneric : _getResolveFunctionMethod)
            .MakeGenericMethod(parameter.ParameterType).Invoke(injector, [parameter]) as Delegate)?.Method;
}