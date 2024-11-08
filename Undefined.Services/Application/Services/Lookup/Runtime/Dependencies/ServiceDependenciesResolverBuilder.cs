using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using Undefined.Services.Exceptions;

namespace Undefined.Services.Application.Services.Lookup.Runtime.Dependencies;

internal class ServiceDependenciesResolverBuilder : IServiceDependenciesResolverBuilder
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ServiceDependencyResolver[] _injectors;

    private readonly MethodInfo _getResolveFunctionMethod =
        typeof(ServiceDependencyResolver).GetMethod(ServiceDependencyResolver.GET_DEPENDENCY_RESOLVER_METHOD_NAME,
            BindingFlags.Public | BindingFlags.Instance)!;

    private readonly MethodInfo _getResolveFunctionMethodGeneric =
        typeof(ServiceDependencyResolver<>).GetMethod(ServiceDependencyResolver.GET_DEPENDENCY_RESOLVER_METHOD_NAME,
            BindingFlags.Public | BindingFlags.Instance)!;

    private readonly ConcurrentDictionary<ServiceId, Action<CoreScope, IService>> _cachedResolvers = [];

    internal ServiceDependenciesResolverBuilder(ServiceProvider serviceProvider,
        ICollection<ServiceDependencyResolver> injectors)
    {
        _serviceProvider = serviceProvider;
        _injectors = new ServiceDependencyResolver[injectors.Count];
        injectors.CopyTo(_injectors, 0);
        for (var i = 0; i < injectors.Count; i++)
        {
            var injector = _injectors[i];
            if(injector.BaseDependencyType is not { } bt) continue;
            if (bt.IsGenericType)
                throw new DependencyResolveException("Generic dependency resolvers are not supported.");
        }
    }


    public Action<CoreScope, IService> GetCachedOrCreateResolver(ServiceId serviceId, Type implementationType) =>
        _cachedResolvers.GetOrAdd(serviceId, _ => CreateResolverNoCache(serviceId, implementationType));

    public Action<CoreScope, IService> CreateResolverNoCache(ServiceId serviceId, Type implementationType)
    {
        var method = new DynamicMethod($"service_{serviceId}_dep_resolve", typeof(void),
            [typeof(CoreScope), typeof(IService)]);
        var generator = method.GetILGenerator();
        var methods = implementationType.GetMethods(BindingFlags.Instance);
        foreach (var m in methods)
        {
            if (m.GetCustomAttribute<ResolverAttribute>() is null) continue;
            if(!m.IsPrivate) throw new InvalidOperationException($"Method {m.Name} has {nameof(ResolverAttribute)} but is not private.");
            
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

    private void ApplyInjector(ServiceDependencyResolver injector, FieldInfo field, ILGenerator generator)
    {
        if (injector.BaseDependencyType is { } baseInjectionType &&
            !baseInjectionType.IsAssignableFrom(field.FieldType)) return;
        var injectorType = injector.GetType();
        if (GetResolveMethod(injectorType.IsGenericType, field, injector) is not { } resolveMethod) return;

        //Call injector resolve function    (Func<CoreScope, IService, TField>)
        generator.Emit(OpCodes.Ldarg_0);
        generator.Emit(OpCodes.Ldarg_1);
        generator.Emit(OpCodes.Call, resolveMethod);
        generator.Emit(OpCodes.Stfld, field);
    }

    private MethodInfo? GetResolveMethod(bool isGeneric, FieldInfo field, ServiceDependencyResolver injector) =>
        ((isGeneric ? _getResolveFunctionMethodGeneric : _getResolveFunctionMethod)
            .MakeGenericMethod(field.FieldType).Invoke(injector, [field]) as Delegate)?.Method;
}