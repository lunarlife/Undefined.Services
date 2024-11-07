using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace Undefined.Services.Application.Services.Lookup.Runtime.Injection;

internal class ServiceRuntimeInjector : IServiceRuntimeInjector
{
    private readonly ServiceInjectorBase[] _injectors;

    private readonly MethodInfo _getResolveFunctionMethod =
        typeof(ServiceInjectorBase).GetMethod(ServiceInjector.GET_RESOLVE_FUNCTION_NAME,
            BindingFlags.Public | BindingFlags.Instance)!;

    private readonly MethodInfo _getResolveFunctionMethodGeneric =
        typeof(ServiceInjector<>).GetMethod(ServiceInjector.GET_RESOLVE_FUNCTION_NAME,
            BindingFlags.Public | BindingFlags.Instance)!;

    private readonly ConcurrentDictionary<ServiceId, Action<CoreScope, IService>> _cachedResolvers = [];

    internal ServiceRuntimeInjector(ICollection<ServiceInjectorBase> injectors)
    {
        _injectors = new ServiceInjectorBase[injectors.Count];
        injectors.CopyTo(_injectors, 0);
    }


    public Action<CoreScope, IService> GetCachedOrCreateInjector(ServiceId serviceId, Type implementationType) =>
        _cachedResolvers.GetOrAdd(serviceId, _ => CreateResolverNoCache(serviceId, implementationType));

    public Action<CoreScope, IService> CreateResolverNoCache(ServiceId serviceId, Type implementationType)
    {
        var method = new DynamicMethod($"service_{serviceId}_resolve", typeof(void),
            [typeof(CoreScope), typeof(IService)]);
        var generator = method.GetILGenerator();
        var fields = implementationType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
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

    private void ApplyInjector(ServiceInjectorBase injector, FieldInfo field, ILGenerator generator)
    {
        if (injector.BaseInjectionType is { } baseInjectionType &&
            !baseInjectionType.IsAssignableFrom(field.FieldType)) return;
        var injectorType = injector.GetType();
        if (GetResolveMethod(injectorType.IsGenericType, field, injector) is not { } resolveMethod) return;

        //Call injector resolve function    (Func<CoreScope, IService, TField>)
        generator.Emit(OpCodes.Ldarg_0);
        generator.Emit(OpCodes.Ldarg_1);
        generator.Emit(OpCodes.Call, resolveMethod);
        generator.Emit(OpCodes.Stfld, field);
    }

    private MethodInfo? GetResolveMethod(bool isGeneric, FieldInfo field, ServiceInjectorBase injector) =>
        ((isGeneric ? _getResolveFunctionMethodGeneric : _getResolveFunctionMethod)
            .MakeGenericMethod(field.FieldType).Invoke(injector, [field]) as Delegate)?.Method;
}